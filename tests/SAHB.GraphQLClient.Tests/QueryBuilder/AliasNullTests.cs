using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryBuilder;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryBuilder
{
    public class AliasNullTests
    {
        private readonly IGraphQLQueryBuilderFromFields _queryBuilder;
        public AliasNullTests()
        {
            _queryBuilder = new GraphQLQueryBuilderFromFields();
        }

        [Fact]
        public void Test_QueryBuilder_Alias_Null()
        {
            var field = new GraphQLField(alias: null, field: "Field1", fields: null, arguments: null);

            var expected = "{\"query\":\"query{Field1}\"}";

            var actual = _queryBuilder.GetQuery(new[] { field });

            Assert.Equal(expected, actual);
        }
    }
}
