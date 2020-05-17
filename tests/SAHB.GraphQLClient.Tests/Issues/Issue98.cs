using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.Issues
{
    public class Issue98
    {
        private readonly IGraphQLQueryGeneratorFromFields queryGenerator = new GraphQLQueryGeneratorFromFields();
        private readonly IGraphQLFieldBuilder fieldBuilder = new GraphQLFieldBuilder();

        [Fact]
        public void Generates_Expected_Query()
        {
            // Arrange
            var expectedQuery = "{\"query\":\"query($foo:FooNumber){foobeta(foo:$foo){id}}\",\"variables\":{\"foo\":{\"fooNumber\":\"123456\",\"systemCode\":\"BAZ\"}}}";

            // Act
            var selectionSet = fieldBuilder.GenerateSelectionSet(typeof(FooQuery));
            var actualQuery = queryGenerator.GenerateQuery(
                GraphQLOperationType.Query,
                selectionSet,
                new GraphQLQueryArgument("foo", new FooNumber { Number = "123456", SystemCode = FooSystemType.BAZ }));

            // Assert
            Assert.Equal(expectedQuery, actualQuery);
        }

        public class FooNumber
        {
            [JsonProperty("fooNumber")]
            public string Number { get; set; }

            [JsonProperty("systemCode")]
            public FooSystemType SystemCode { get; set; }
        }

        public class FooQuery
        {
            [GraphQLArguments("foo", "FooNumber", "foo")]
            public FooBeta FooBeta { get; set; }
        }

        [GraphQLFieldName("foobeta")]
        public class FooBeta : Foo
        {
        }

        public class Foo
        {
            public string Id { get; set; }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum FooSystemType
        {
            [EnumMember(Value = "BAZ")]
            BAZ,
        }
    }
}
