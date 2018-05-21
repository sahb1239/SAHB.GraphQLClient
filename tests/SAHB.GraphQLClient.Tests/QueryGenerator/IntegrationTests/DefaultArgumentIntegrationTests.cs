using SAHB.GraphQLClient.Extentions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator.IntegrationTests
{
    public class DefaultArgumentIntegrationTests
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public DefaultArgumentIntegrationTests()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        public class QueryWithArgumentDefaultValue
        {
            [GraphQLArguments("argumentName1", "argumentType1", "variableName1", true, true, 1)]
            public string Field { get; set; }
        }

        public class QueryWithArgumentMultipleDefaultValue
        {
            [GraphQLArguments("argumentName1", "argumentType1", "variableName1", true, true, 1)]
            [GraphQLArguments("argumentName2", "argumentType1", "variableName2", true, true, 2)]
            public string Field { get; set; }
        }

        [Fact]
        public void Test_Query_Default_Value()
        {
            var expected =
                "{\"query\":\"query{field(argumentName1:1)}\"}";

            var actual = _queryGenerator.GetQuery<QueryWithArgumentDefaultValue>(_fieldBuilder);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Query_Multiple_Default_Value()
        {
            var expected =
                "{\"query\":\"query{field(argumentName1:1 argumentName2:2)}\"}";

            var actual = _queryGenerator.GetQuery<QueryWithArgumentMultipleDefaultValue>(_fieldBuilder);

            Assert.Equal(expected, actual);
        }
    }
}
