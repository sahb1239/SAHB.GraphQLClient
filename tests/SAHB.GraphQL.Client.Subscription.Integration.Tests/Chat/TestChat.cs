using GraphQL.Types;
using SAHB.GraphQLClient.Subscription.Integration.Tests.TestServer;
using SAHB.GraphQLClient;
using System.Threading.Tasks;
using Xunit;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQL.Client.Subscription.Integration.Tests.ChatSchema;
using System;
using SAHB.GraphQL.Client.Subscription;
using System.Threading;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using System.Collections.Generic;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Subscription.Integration.Tests
{
    public class TestChat : IClassFixture<GraphQLWebApplicationFactory<ChatSchema>>
    {
        private readonly GraphQLWebApplicationFactory<ChatSchema> _factory;

        public TestChat(GraphQLWebApplicationFactory<ChatSchema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();

            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Create websocket GraphQL client
            var wsClient = _factory.Server.CreateWebSocketClient();
            //wsClient.SubProtocols.Add("graphql-ws");
            wsClient.ConfigureRequest = request => request.Headers.Add("Sec-WebSocket-Protocol", "graphql-ws");

            var wsUri = new UriBuilder(_factory.Server.BaseAddress)
            {
                Scheme = "wss",
                Path = "graphql"
            }.Uri;

            var websocket = await wsClient.ConnectAsync(wsUri, tokenSource.Token);

            var graphQLSubscriptionClient = new GraphQLSubscriptionClient(websocket, tokenSource.Token, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
            await graphQLSubscriptionClient.Connect();

            // Act
            // Registrer operation
            var operation = await graphQLSubscriptionClient.ExecuteOperation<MessageSubscription>();

            List<GraphQLDataResult<MessageSubscription>> recievedData = new List<GraphQLDataResult<MessageSubscription>>();
            operation.DataRecieved += (sender, e) =>
            {
                recievedData.Add(e.ReceivedData);
            };
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Send message
            await graphQLClient.CreateMutation<SendMessageMutation>("http://localhost/graphql", arguments: new GraphQLQueryArgument("message", new MessageInputType
            {
                content = "Message 1",
                fromId = "SAHB",
                sentAt = DateTime.Now.AddDays(-1)
            })).Execute();

            await Task.Delay(TimeSpan.FromSeconds(1));

            // Assert
            Assert.Single(recievedData);
        }

        public class MessageSubscription
        {
            public Message MessageAdded { get; set; }
        }

        public class Message
        {
            public Author From { get; set; }
            public string Content { get; set; }
            public DateTime SentAt { get; set; }
        }

        public class Author
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
        }

        public class SendMessageMutation
        {
            public AddMessage AddMessage { get; set; }
        }

        [GraphQLArguments("message", "MessageInputType", "message", isRequired: true)]
        public class AddMessage
        {
            public string Content { get; set; }
        }

        public class MessageInputType
        {
            public string fromId { get; set; }
            public string content { get; set; }
            public DateTime? sentAt { get; set; }
        }
    }
}
