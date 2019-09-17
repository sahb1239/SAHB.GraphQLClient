using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Subscription.Internal;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Subscription
{
    /// <inheritdoc />
    public class GraphQLSubscriptionClient : IGraphQLSubscriptionClient
    {
        private long _operationCounter = 1;
        private readonly object _locker = new object();
        private readonly ConcurrentDictionary<string, GraphQLOperationSource> _operations = new ConcurrentDictionary<string, GraphQLOperationSource>();
        private readonly ConcurrentDictionary<string, OperationMessage> _operationMessages = new ConcurrentDictionary<string, OperationMessage>();

        private const int ReceiveChunkSize = 1024;
        private const int SendChunkSize = 1024;

        private readonly CancellationToken _cancellationToken;
        private readonly byte[] buffer = new byte[ReceiveChunkSize];

        public event EventHandler Disconnected;

        public GraphQLSubscriptionClient(WebSocket webSocket, CancellationToken cancellationToken) 
            : this(webSocket, cancellationToken, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), 
                  new GraphQLDeserilization())
        {

        }

        public GraphQLSubscriptionClient(WebSocket webSocket, CancellationToken cancellationToken, IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLDeserialization deserialization)
        {
            WebSocket = webSocket;
            FieldBuilder = fieldBuilder;
            QueryGenerator = queryGenerator;
            Deserialization = deserialization;
            _cancellationToken = cancellationToken;
        }

        public WebSocket WebSocket { get; }
        public IGraphQLFieldBuilder FieldBuilder { get; }
        public IGraphQLQueryGeneratorFromFields QueryGenerator { get; }
        public IGraphQLDeserialization Deserialization { get; }

        /// <inheritdoc />
        public bool IsConnected => WebSocket.State == WebSocketState.Open;

        /// <inheritdoc />
        public bool IsInitilized { get; private set; }

        /// <inheritdoc />
        public async Task Initilize()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Connection is not open");

            // Sent GQL_CONNECTION_INIT
            await SendOperationMessage(new OperationMessage
            {
                Type = MessageType.GQL_CONNECTION_INIT,
                Payload = new object()
            }).ConfigureAwait(false);

            // Wait for ack
            string ackMessage = await ReadMessage().ConfigureAwait(false);

            var serverAck = JsonConvert.DeserializeObject<OperationMessage>(ackMessage);
            if (serverAck.Type != MessageType.GQL_CONNECTION_ACK)
            {
                throw new NotImplementedException("Unknown websocket response: " + serverAck.Type);
            }

            // Start listening into new task
            await Task.Factory.StartNew(() => StartListen()).ConfigureAwait(false);

            // Set initilized
            IsInitilized = true;
        }

        /// <inheritdoc />
        public async Task<IGraphQLSubscriptionOperation<T>> ExecuteOperation<T>(params GraphQLQueryArgument[] arguments) where T : class
        {
            if (!IsConnected)
                throw new InvalidOperationException("Websocket is not connected");

            if (!IsInitilized)
                throw new InvalidOperationException("GraphQLSubscriptionClient is not initilized");

            // Get operationId
            long operationId;
            lock (_locker)
            {
                operationId = _operationCounter++;
            }

            // Get query
            var selectionSet = FieldBuilder.GenerateSelectionSet(typeof(T));
            var query = QueryGenerator.GenerateQuery(GraphQLOperationType.Subscription, selectionSet, arguments);

            // Generate OperationMessage for starting the operation
            var message = new OperationMessage
            {
                Id = operationId.ToString(),
                Type = MessageType.GQL_START,
                Payload = JsonConvert.DeserializeObject(query)
            };

            // Generate stop message
            var stopMessage = new OperationMessage
            {
                Id = operationId.ToString(),
                Type = MessageType.GQL_STOP
            };

            // Create GraphQLOperationSource
            var operationSource = new GraphQLOperationSource(() =>
            {
                // Generate stop
                return SendOperationMessage(stopMessage);
            });

            // Add to list
            var result = _operations.TryAdd(operationId.ToString(), operationSource);
            result &= _operationMessages.TryAdd(operationId.ToString(), message);
            if (!result)
            {
                throw new InvalidOperationException("OperationId does already exist in operation messages. This should never happen - please report this as a bug");
            }

            // Create IGraphQLSubscriptionOperation
            var subscription = new GraphQLSubscriptionOperation<T>(operationSource, selectionSet, Deserialization);

            // Send subscribe message
            await SendOperationMessage(message).ConfigureAwait(false);

            // Return the subscription
            return subscription;
        }

        /// <inheritdoc />
        public async Task RestartActiveGraphQLOperations()
        {
            // Get copy of operationMessages
            var operationMessagesCopy = _operationMessages.ToList().Select(e => e.Value);

            // Resend each operationMessage
            foreach (var message in operationMessagesCopy)
            {
                await SendOperationMessage(message).ConfigureAwait(false);
            }
        }

        private void OnOperationRecieved(OperationMessage operationMessage)
        {
            if (operationMessage == null)
                return;

            // Find id
            var source = _operations[operationMessage.Id];

            switch (operationMessage.Type)
            {
                case MessageType.GQL_DATA:
                case MessageType.GQL_ERROR:
                    source.HandlePayload(operationMessage.Payload as JObject);
                    break;
                case MessageType.GQL_COMPLETE:
                    source.HandleCompleted();

                    _operations.TryRemove(operationMessage.Id, out _);
                    _operationMessages.TryRemove(operationMessage.Id, out _);

                    break;
                default:
                    Logger?.LogWarning($"Message not handled:{Environment.NewLine}{operationMessage}");
                    // TODO: Handle the opration type
                    break;
            }
        }

        private void OnMessageRecieved(string message)
        {
            Logger?.LogInformation($"Message recieved:{Environment.NewLine}{message}");

            OnOperationRecieved(JsonConvert.DeserializeObject<OperationMessage>(message));
        }

        private void OnDisconnected()
        {
            Disconnected?.Invoke(this, new EventArgs());
        }

        private async Task SendOperationMessage(OperationMessage operationMessage)
        {
            string message = JsonConvert.SerializeObject(operationMessage);
            await SendMessageAsync(message).ConfigureAwait(false);
        }

        private async Task<string> ReadMessage()
        {
            var stringResult = new StringBuilder();

            WebSocketReceiveResult result;
            do
            {
                result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken).ConfigureAwait(false);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await
                        WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None).ConfigureAwait(false);
                    OnDisconnected();
                }
                else
                {
                    var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    stringResult.Append(str);
                }

            } while (!result.EndOfMessage);

            return stringResult.ToString();
        }

        private async Task StartListen()
        {
            try
            {
                while (WebSocket.State == WebSocketState.Open)
                {
                    var message = await ReadMessage().ConfigureAwait(false);

                    try
                    {
                        OnMessageRecieved(message);
                    }
                    catch (Exception ex)
                    {
                        Logger?.LogError(new EventId(2), ex, "Exception handling message");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(new EventId(1), ex, "Exception from websocket client");
            }
            finally
            {
                OnDisconnected();
            }
        }

        private async Task SendMessageAsync(string message)
        {
            if (WebSocket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("Connection is not open.");
            }

            Logger?.LogInformation($"Sending message:{Environment.NewLine}{message}");

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = (SendChunkSize * i);
                var count = SendChunkSize;
                var lastMessage = ((i + 1) == messagesCount);

                if ((count * (i + 1)) > messageBuffer.Length)
                {
                    count = messageBuffer.Length - offset;
                }

                await WebSocket.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, _cancellationToken).ConfigureAwait(false);
            }
        }

        #region Logging

        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// Contains a logger factory for the GraphQLHttpClient
        /// </summary>
        public ILoggerFactory LoggerFactory
        {
            internal get { return _loggerFactory; }
            set
            {
                _loggerFactory = value;
                if (_loggerFactory != null)
                {
                    Logger = _loggerFactory.CreateLogger<GraphQLHttpClient>();
                }
            }
        }

        /// <summary>
        /// Contains the logger for the class
        /// </summary>
        private ILogger<GraphQLHttpClient> Logger { get; set; }

        #endregion
    }
}
