using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Deserialization;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Subscription
{

    // Inspired by https://gist.github.com/xamlmonkey/4737291
    public class GraphQLSubscriptionWebSocketClient : IGraphQLSubscriptionWebSocketClient
    {
        private readonly ClientWebSocket _webSocket;

        private readonly CancellationToken _cancellationToken;

        private readonly IGraphQLFieldBuilder fieldBuilder;
        private readonly IGraphQLQueryGeneratorFromFields queryGenerator;
        private readonly IGraphQLDeserialization deserialization;

        public bool IsConnected => _webSocket.State == WebSocketState.Open;

        public GraphQLSubscriptionWebSocketClient(IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLDeserialization deserialization, CancellationToken cancellationToken) :
           this(new ClientWebSocket(), fieldBuilder, queryGenerator, deserialization, cancellationToken)
        {
        }

        public GraphQLSubscriptionWebSocketClient(ClientWebSocket websocket, IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLDeserialization deserialization, CancellationToken cancellationToken)
        {
            _webSocket = websocket;

            _cancellationToken = cancellationToken;
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
            return new GraphQLSubscriptionWebSocketClient(new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization(), CancellationToken.None);
        }

        /// <summary>
        /// Initilizes a new instance of GraphQL subscription client (using the specified <see cref="ClientWebSocket"/>) which supports generating GraphQL subscriptions from a <see cref="Type"/>
        /// </summary>
        /// <returns>A new instance of the GraphQL subscription client</returns>
        public static IGraphQLSubscriptionWebSocketClient Default(ClientWebSocket websocket)
        {
            return new GraphQLSubscriptionWebSocketClient(websocket, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization(), CancellationToken.None);
        }

        public async Task<GraphQLSubscriptionClient> Connect(Uri url)
        {
            if (_webSocket.State == WebSocketState.Open)
                throw new InvalidOperationException("Connection is already open");

            // Connect to web socket
            await _webSocket.ConnectAsync(url, _cancellationToken).ConfigureAwait(false);

            // Create client
            return new GraphQLSubscriptionClient(_webSocket, _cancellationToken, fieldBuilder, queryGenerator, deserialization);
        }

        public async Task Disconnect()
        {
            if (_webSocket != null)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, _cancellationToken).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _webSocket.Dispose();
        }
    }
}
