using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.QueryGenerator
{
    public class SubscriptionTests
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;

        public SubscriptionTests()
        {
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        [Fact]
        public void TestSubscriptionQueryGeneration()
        {
            // Arrange
            var operation = new GraphQLOperation(GraphQLOperationType.Subscription, new List<GraphQLField>()
            {
                new GraphQLField(null, "newMessage", new List<GraphQLField> {
                    new GraphQLField(null, "body", null, null),
                    new GraphQLField(null, "sender", null, null)
                }, null)
            });

            string expected = "{\"query\":\"subscription newMessage{newMessage{body sender}}\"}";

            // Act
            var actual = _queryGenerator.GenerateQuery(operation);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
