using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAHB.GraphQL.Client.Deserialization;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Result;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SAHB.GraphQL.Client.Subscription
{

    // Inspired by https://gist.github.com/xamlmonkey/4737291
    public class GraphQLSubscriptionWebSocketClient : IGraphQLSubscriptionWebSocketClient
    {
        private readonly ClientWebSocket _webSocket = new ClientWebSocket();

        private readonly Dictionary<string, GraphQLOperationSource> _operations = new Dictionary<string, GraphQLOperationSource>();
        private long _counter = 1;

        private const int ReceiveChunkSize = 1024;
        private const int SendChunkSize = 1024;

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly CancellationToken _cancellationToken;

        private readonly IGraphQLFieldBuilder fieldBuilder;
        private readonly IGraphQLQueryGeneratorFromFields queryGenerator;
        private readonly IGraphQLDeserialization deserialization;

        byte[] buffer = new byte[ReceiveChunkSize];

        public bool IsConnected => _webSocket.State == WebSocketState.Open;

        public GraphQLSubscriptionWebSocketClient(IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLDeserialization deserialization)
        {
            _cancellationToken = _tokenSource.Token;
            _webSocket.Options.AddSubProtocol("graphql-ws");
            this.fieldBuilder = fieldBuilder;
            this.queryGenerator = queryGenerator;
            this.deserialization = deserialization;
        }

        /// <summary>
        /// Initilizes a new instance of GraphQL subscription client which supports generating GraphQL subscriptions from a <see cref="Type"/>
        /// </summary>
        /// <returns>A new instance of the GraphQL subscription client</returns>
        public static IGraphQLSubscriptionWebSocketClient Default()
        {
            return new GraphQLSubscriptionWebSocketClient(new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
        }

        public async Task<bool> Connect(Uri url)
        {
            if (_webSocket.State == WebSocketState.Open)
                throw new InvalidOperationException("Connection is already open");

            // Connect to web socket
            await _webSocket.ConnectAsync(url, _cancellationToken);

            if (_webSocket.State == WebSocketState.Open)
            {
                // Sent GQL_CONNECTION_INIT
                await SendOperationMessage(new OperationMessage
                {
                    Type = MessageType.GQL_CONNECTION_INIT,
                    Payload = new object()
                });

                // Wait for ack
                var ackMessage = await ReadMessage();
                var serverAck = JsonConvert.DeserializeObject<OperationMessage>(ackMessage);
                if (serverAck.Type != MessageType.GQL_CONNECTION_ACK)
                {
                    throw new NotImplementedException("Unknown websocket response: " + serverAck.Type);
                }

                // Start listening into new task
                await Task.Factory.StartNew(() => StartListen());
            }

            return _webSocket.State == WebSocketState.Open;
        }

        public async Task Disconnect()
        {
            if (_webSocket != null)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _cancellationToken);
            }
        }

        public async Task<IGraphQLSubscriptionOperation<T>> ExecuteOperation<T>(params GraphQLQueryArgument[] arguments) where T : class
        {
            // Get operationId
            var operationId = _counter++;
            var operationType = MessageType.GQL_START;

            // Get query
            var operation = fieldBuilder.GenerateOperation(typeof(T), GraphQLOperationType.Subscription);
            var query = queryGenerator.GenerateQuery(operation, arguments);

            // Generate OperationMessage
            var message = new OperationMessage
            {
                Id = operationId.ToString(),
                Type = operationType,
                Payload = JsonConvert.DeserializeObject(query)
            };

            // Generate stop message
            var stopMessage = new OperationMessage
            {
                Id = operationId.ToString()
            };

            // Create GraphQLOperationSource
            var operationSource = new GraphQLOperationSource(() =>
            {
                // Generate stop
                return SendOperationMessage(stopMessage);
            });

            // Create IGraphQLSubscriptionOperation
            var subscription = new GraphQLSubscriptionOperation<T>(operationSource, operation, deserialization);

            // Add to list
            _operations.Add(operationId.ToString(), operationSource);

            // Send subscribe message
            await SendOperationMessage(message);

            // Return the subscription
            return subscription;
        }

        public void Dispose()
        {
            _webSocket.Dispose();
        }

        private void OnOperationRecieved(OperationMessage operationMessage)
        {
            if (operationMessage == null)
                return;

            switch(operationMessage.Type)
            {
                case MessageType.GQL_DATA:
                case MessageType.GQL_ERROR:
                    // Find id
                    var source = _operations[operationMessage.Id];

                    source.HandlePayload(operationMessage.Payload as JObject);
                    break;
                default:
                    // TODO: Handle the opration type
                    break;
            }
        }

        private void OnMessageRecieved(string message)
        {
            OnOperationRecieved(JsonConvert.DeserializeObject<OperationMessage>(message));
        }

        private void OnDisconnected()
        {
            
        }

        private async Task SendOperationMessage(OperationMessage operationMessage)
        {
            string message = JsonConvert.SerializeObject(operationMessage);
            await SendMessageAsync(message);
        }

        private async Task<string> ReadMessage()
        {
            var stringResult = new StringBuilder();

            WebSocketReceiveResult result;
            do
            {
                result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await
                        _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
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
                while (_webSocket.State == WebSocketState.Open)
                {
                    var message = await ReadMessage();

                    OnMessageRecieved(message);
                }
            }
            catch (Exception)
            {
                OnDisconnected();
            }
        }

        private async Task SendMessageAsync(string message)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

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

                await _webSocket.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, _cancellationToken);
            }
        }
    }
}
