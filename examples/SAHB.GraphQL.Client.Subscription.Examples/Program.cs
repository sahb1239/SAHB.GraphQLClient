using SAHB.GraphQL.Client.Deserialization;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Threading.Tasks;

namespace SAHB.GraphQL.Client.Subscription.Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (IGraphQLSubscriptionWebSocketClient wssClient = GraphQLSubscriptionWebSocketClient.Default())
            {
                if (!await wssClient.Connect(new Uri("ws://localhost:60340/graphql")))
                {
                    Console.WriteLine("Could not connect!");
                    Console.ReadKey();

                    return;
                }

                var operation = await wssClient.ExecuteOperation<MessageSubscription>();
                operation.DataRecieved += (sender, e) =>
                {
                    Console.WriteLine("Opration 1: " + e.ReceivedData.Data.MessageAdded.From.Id + ": " + e.ReceivedData.Data.MessageAdded.Content);
                };

                var operation2 = await wssClient.ExecuteOperation<MessageSubscription>();
                operation2.DataRecieved += (sender, e) =>
                {
                    Console.WriteLine("Operation 2: " + e.ReceivedData.Data.MessageAdded.From.Id + ": " + e.ReceivedData.Data.MessageAdded.Content);
                };

                var operation3 = await wssClient.ExecuteOperation<MessageSubscription>();
                operation3.DataRecieved += (sender, e) =>
                {
                    Console.WriteLine("Operation 3: " + e.ReceivedData.Data.MessageAdded.From.Id + ": " + e.ReceivedData.Data.MessageAdded.Content);
                };

                // Send message
                var client = GraphQLHttpClient.Default();
                await client.CreateMutation<SendMessageMutation>("http://localhost:60340/graphql", arguments: new GraphQLQueryArgument("message", new MessageInputType
                {
                    content = "Message 1",
                    fromId = "SAHB",
                    sentAt = DateTime.Now.AddDays(-1)
                })).Execute();

                await operation.Stop();

                Console.ReadKey();

                await client.CreateMutation<SendMessageMutation>("http://localhost:60340/graphql", arguments: new GraphQLQueryArgument("message", new MessageInputType
                {
                    content = "Message 2",
                    fromId = "SAHB"
                })).Execute();

                Console.ReadKey();

                await wssClient.Disconnect();

                Console.ReadKey();
            }
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
