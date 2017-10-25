using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryBuilder;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryBuilder
{
    public class QueryBuilderTest
    {
        private readonly GraphQLQueryBuilder _queryBuilder;

        public QueryBuilderTest()
        {
            _queryBuilder = new GraphQLQueryBuilder(null);
        }

        [Fact]
        public void Check_Simple_Query_Single_Field()
        {
            var fields = new[]
            {
                new GraphQLField("alias", "field", null, null, null, null),
            };
            var expected = "{\"query\":\"query{alias:field}\"}";

            var actual = _queryBuilder.GetQuery(fields);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Check_Simple_Query_Multiple_Field()
        {
            var fields = new[]
            {
                new GraphQLField("alias", "field", null, null, null, null),
                new GraphQLField("alias2", "field2", null, null, null, null),
            };
            var expected = "{\"query\":\"query{alias:field alias2:field2}\"}";

            var actual = _queryBuilder.GetQuery(fields);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Check_Simple_Query_Nested_Field()
        {
            var fields = new[]
            {
                new GraphQLField(
                    alias: "alias", 
                    field: "field", 
                    fields: new []
                    {
                        new GraphQLField(
                            alias: "alias2", 
                            field: "field2", 
                            fields: null, 
                            arguments: null, 
                            type: null, 
                            propertyInfo: null
                        ), 
                    }, 
                    arguments: null, 
                    type: null, 
                    propertyInfo: null),
            };
            var expected = "{\"query\":\"query{alias:field{alias2:field2}}\"}";

            var actual = _queryBuilder.GetQuery(fields);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Check_Simple_Query_Multiple_Nested_Field()
        {
            var fields = new[]
            {
                new GraphQLField(
                    alias: "alias",
                    field: "field",
                    fields: new []
                    {
                        new GraphQLField(
                            alias: "alias2",
                            field: "field2",
                            fields: null,
                            arguments: null,
                            type: null,
                            propertyInfo: null
                        ),
                        new GraphQLField(
                            alias: "alias3",
                            field: "field3",
                            fields: null,
                            arguments: null,
                            type: null,
                            propertyInfo: null
                        ),
                    },
                    arguments: null,
                    type: null,
                    propertyInfo: null),
                new GraphQLField(
                    alias: "alias4",
                    field: "field4",
                    fields: null,
                    arguments: null,
                    type: null,
                    propertyInfo: null
                )
            };
            var expected = "{\"query\":\"query{alias:field{alias2:field2 alias3:field3} alias4:field4}\"}";

            var actual = _queryBuilder.GetQuery(fields);

            Assert.Equal(expected, actual);
        }
    }
}
