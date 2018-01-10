using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator
{
    public class AliasNullTests
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        public AliasNullTests()
        {
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        [Fact]
        public void Test_QueryGenerator_Alias_Null()
        {
            var field = new GraphQLField(alias: null, field: "Field1", fields: null, arguments: null);

            var expected = "{\"query\":\"query{Field1}\"}";

            var actual = _queryGenerator.GetQuery(new[] { field });

            Assert.Equal(expected, actual);
        }
    }
}
