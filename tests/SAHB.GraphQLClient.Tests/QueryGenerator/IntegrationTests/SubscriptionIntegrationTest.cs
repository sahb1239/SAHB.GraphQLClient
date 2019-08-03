using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator.IntegrationTests
{
    public class SubscriptionIntegrationTests
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public SubscriptionIntegrationTests()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }
        
        public class MessageSubscription
        {
            public Message newMessage { get; set; }
        }

        public class Message
        {
            public string Body { get; set; }
            public string Sender { get; set; }
        }

        [Fact]
        public void TestSubscriptionQueryGeneration()
        {
            // Arrange
            var selectionSet = _fieldBuilder.GenerateSelectionSet(typeof(MessageSubscription));

            string expected = "{\"query\":\"subscription newMessage{newMessage{body sender}}\"}";

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Subscription, selectionSet);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
