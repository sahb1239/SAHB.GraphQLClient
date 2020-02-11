using Newtonsoft.Json;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using System;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.Issues
{
    public class Issue77
    {
        private readonly IGraphQLQueryGeneratorFromFields queryGenerator = new GraphQLQueryGeneratorFromFields();
        private readonly IGraphQLFieldBuilder fieldBuilder = new GraphQLFieldBuilder();

        [Fact]
        public void Generates_Expected_Query()
        {
            // Arrange
            var expectedQuery = "{\"query\":\"subscription subRd{subRd(src:SB){src record}}\"}";

            // Act
            var selectionSet = fieldBuilder.GenerateSelectionSet(typeof(MessageSubscription));
            var actualQuery = queryGenerator.GenerateQuery(
                GraphQLOperationType.Subscription,
                selectionSet,
                new GraphQLQueryArgument("fromSrc", RdSrc.SB));

            // Assert
            Assert.Equal(expectedQuery, actualQuery);
        }

        private class MessageSubscription
        {
            [GraphQLArguments("src", "RdSrc!", "fromSrc", isRequired: true, inlineArgument: true)]
            public Message subRd { get; set; }
        }

        private class Message
        {
            public string src { get; set; }
            public string record { get; set; }
        }

        private enum RdSrc
        {
            SB,
            XB
        }
    }
}
