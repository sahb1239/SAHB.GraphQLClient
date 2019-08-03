using SAHB.GraphQLClient.Subscription.Integration.Tests.TestServer;
using System.Threading.Tasks;
using Xunit;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQL.Client.Subscription.Integration.Tests.ChatSchema;
using System;
using System.Threading;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using System.Collections.Generic;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Subscription.Integration.Tests
{
    public class TestChat
    {
        private readonly GraphQLWebApplicationFactory<ChatSchema> _factory;

        public TestChat()
        {
            _factory = new GraphQLWebApplicationFactory<ChatSchema>();
        }

        [Fact]
        public async Task Test_Recieve_Message()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();

            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Create websocket GraphQL client
            var wsClient = _factory.Server.CreateWebSocketClient();
            wsClient.ConfigureRequest = request => request.Headers.Add("Sec-WebSocket-Protocol", "graphql-ws");

            var wsUri = new UriBuilder(_factory.Server.BaseAddress)
            {
                Scheme = "wss",
                Path = "graphql"
            }.Uri;

            var websocket = await wsClient.ConnectAsync(wsUri, tokenSource.Token);

            var graphQLSubscriptionClient = new GraphQLSubscriptionClient(websocket, tokenSource.Token, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
            await graphQLSubscriptionClient.Initilize();

            // Act
            // Registrer operation
            var operation = await graphQLSubscriptionClient.ExecuteOperation<MessageAddedSubscription>();

            List<GraphQLDataResult<MessageAddedSubscription>> recievedData = new List<GraphQLDataResult<MessageAddedSubscription>>();
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

            await Task.Delay(TimeSpan.FromSeconds(2));

            // Assert
            Assert.Single(recievedData);
        }

        [Fact]
        public async Task Test_Recieve_Multiple_Messages()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();

            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Create websocket GraphQL client
            var wsClient = _factory.Server.CreateWebSocketClient();
            wsClient.ConfigureRequest = request => request.Headers.Add("Sec-WebSocket-Protocol", "graphql-ws");

            var wsUri = new UriBuilder(_factory.Server.BaseAddress)
            {
                Scheme = "wss",
                Path = "graphql"
            }.Uri;

            var websocket = await wsClient.ConnectAsync(wsUri, tokenSource.Token);

            var graphQLSubscriptionClient = new GraphQLSubscriptionClient(websocket, tokenSource.Token, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
            await graphQLSubscriptionClient.Initilize();

            // Act
            // Registrer operation
            var operation = await graphQLSubscriptionClient.ExecuteOperation<MessageAddedSubscription>();

            List<GraphQLDataResult<MessageAddedSubscription>> recievedData = new List<GraphQLDataResult<MessageAddedSubscription>>();
            operation.DataRecieved += (sender, e) =>
            {
                recievedData.Add(e.ReceivedData);
            };
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Send 2 messages
            await graphQLClient.CreateMutation<SendMessageMutation>("http://localhost/graphql", arguments: new GraphQLQueryArgument("message", new MessageInputType
            {
                content = "Message 1",
                fromId = "SAHB",
                sentAt = DateTime.Now.AddDays(-1)
            })).Execute();
            await graphQLClient.CreateMutation<SendMessageMutation>("http://localhost/graphql", arguments: new GraphQLQueryArgument("message", new MessageInputType
            {
                content = "Message 1",
                fromId = "SAHB",
                sentAt = DateTime.Now.AddDays(-1)
            })).Execute();

            await Task.Delay(TimeSpan.FromSeconds(2));

            // Assert
            Assert.Equal(2, recievedData.Count);
        }

        [Fact]
        public async Task Test_Recieve_Message_By_User()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();

            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Create websocket GraphQL client
            var wsClient = _factory.Server.CreateWebSocketClient();
            wsClient.ConfigureRequest = request => request.Headers.Add("Sec-WebSocket-Protocol", "graphql-ws");

            var wsUri = new UriBuilder(_factory.Server.BaseAddress)
            {
                Scheme = "wss",
                Path = "graphql"
            }.Uri;

            var websocket = await wsClient.ConnectAsync(wsUri, tokenSource.Token);

            var graphQLSubscriptionClient = new GraphQLSubscriptionClient(websocket, tokenSource.Token, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
            await graphQLSubscriptionClient.Initilize();

            // Act
            // Registrer operation
            var operation = await graphQLSubscriptionClient.ExecuteOperation<MessageAddedByUserSubscription>(new GraphQLQueryArgument("fromId", "SAHB"));

            List<GraphQLDataResult<MessageAddedByUserSubscription>> recievedData = new List<GraphQLDataResult<MessageAddedByUserSubscription>>();
            operation.DataRecieved += (sender, e) =>
            {
                recievedData.Add(e.ReceivedData);
            };
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Send 2 message from 2 users only one should be retrieved in subscription
            await graphQLClient.CreateMutation<SendMessageMutation>("http://localhost/graphql", arguments: new GraphQLQueryArgument("message", new MessageInputType
            {
                content = "Message 1",
                fromId = "SAHB",
                sentAt = DateTime.Now.AddDays(-1)
            })).Execute();

            await graphQLClient.CreateMutation<SendMessageMutation>("http://localhost/graphql", arguments: new GraphQLQueryArgument("message", new MessageInputType
            {
                content = "Message 1",
                fromId = "OtherUser",
                sentAt = DateTime.Now.AddDays(-1)
            })).Execute();

            await Task.Delay(TimeSpan.FromSeconds(2));

            // Assert
            Assert.Single(recievedData);
        }

        [Fact]
        public async Task Test_Recieve_Multiple_Subscribers()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();

            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Create websocket GraphQL client
            var wsClient = _factory.Server.CreateWebSocketClient();
            wsClient.ConfigureRequest = request => request.Headers.Add("Sec-WebSocket-Protocol", "graphql-ws");

            var wsUri = new UriBuilder(_factory.Server.BaseAddress)
            {
                Scheme = "wss",
                Path = "graphql"
            }.Uri;

            var websocket = await wsClient.ConnectAsync(wsUri, tokenSource.Token);

            var graphQLSubscriptionClient = new GraphQLSubscriptionClient(websocket, tokenSource.Token, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
            await graphQLSubscriptionClient.Initilize();

            // Act
            // Registrer operations
            var operationByUser = await graphQLSubscriptionClient.ExecuteOperation<MessageAddedByUserSubscription>(new GraphQLQueryArgument("fromId", "SAHB"));

            List<GraphQLDataResult<MessageAddedByUserSubscription>> recievedDataByUser = new List<GraphQLDataResult<MessageAddedByUserSubscription>>();
            operationByUser.DataRecieved += (sender, e) =>
            {
                recievedDataByUser.Add(e.ReceivedData);
            };

            var operation = await graphQLSubscriptionClient.ExecuteOperation<MessageAddedSubscription>();

            List<GraphQLDataResult<MessageAddedSubscription>> recievedData = new List<GraphQLDataResult<MessageAddedSubscription>>();
            operation.DataRecieved += (sender, e) =>
            {
                recievedData.Add(e.ReceivedData);
            };
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Send 2 message from 2 users only one should be retrieved in subscription
            await graphQLClient.CreateMutation<SendMessageMutation>("http://localhost/graphql", arguments: new GraphQLQueryArgument("message", new MessageInputType
            {
                content = "Message 1",
                fromId = "SAHB",
                sentAt = DateTime.Now.AddDays(-1)
            })).Execute();

            await Task.Delay(TimeSpan.FromSeconds(1));

            // Assert
            Assert.Single(recievedDataByUser);
            Assert.Single(recievedData);
        }

        [Fact]
        public async Task Test_Complete()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();

            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Create websocket GraphQL client
            var wsClient = _factory.Server.CreateWebSocketClient();
            wsClient.ConfigureRequest = request => request.Headers.Add("Sec-WebSocket-Protocol", "graphql-ws");

            var wsUri = new UriBuilder(_factory.Server.BaseAddress)
            {
                Scheme = "wss",
                Path = "graphql"
            }.Uri;

            var websocket = await wsClient.ConnectAsync(wsUri, tokenSource.Token);

            var graphQLSubscriptionClient = new GraphQLSubscriptionClient(websocket, tokenSource.Token, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
            await graphQLSubscriptionClient.Initilize();

            // Act
            List<bool> completedTimes = new List<bool>();

            // Registrer operations and stop it
            var operation = await graphQLSubscriptionClient.ExecuteOperation<MessageAddedSubscription>();
            operation.Completed += (sender, e) =>
            {
                completedTimes.Add(true);
            };
            await operation.Stop();

            await Task.Delay(TimeSpan.FromSeconds(2));

            // Assert
            Assert.Single(completedTimes);
        }

        public class MessageAddedSubscription
        {
            public Message MessageAdded { get; set; }
        }

        public class MessageAddedByUserSubscription
        {
            [GraphQLArguments("id", "String", "fromId", IsRequired = true)]
            public Message messageAddedByUser { get; set; }
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
