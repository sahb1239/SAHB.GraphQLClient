using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator
{
    public class QueryGeneratorTest
    {
        [Fact]
        public void Check_Simple_Query_Single_Field()
        {
            var fields = new[]
            {
                new GraphQLField("alias", "field", null, null),
            };
            var fieldBuilder = new FieldBuilderMock(fields);
            var queryGenerator = new GraphQLQueryGenerator(fieldBuilder);
            var expected = "{\"query\":\"query{alias:field}\"}";

            var actual = queryGenerator.GetQuery<string>(); // Typeparameter is ignored since it just returns the fields

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Check_Simple_Query_Multiple_Field()
        {
            var fields = new[]
            {
                new GraphQLField("alias", "field", null, null),
                new GraphQLField("alias2", "field2", null, null),
            };
            var fieldBuilder = new FieldBuilderMock(fields);
            var queryGenerator = new GraphQLQueryGenerator(fieldBuilder);
            var expected = "{\"query\":\"query{alias:field alias2:field2}\"}";

            var actual = queryGenerator.GetQuery<string>(); // Typeparameter is ignored since it just returns the fields

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
                            arguments: null
                        ), 
                    }, 
                    arguments: null),
            };
            var fieldBuilder = new FieldBuilderMock(fields);
            var queryGenerator = new GraphQLQueryGenerator(fieldBuilder);
            var expected = "{\"query\":\"query{alias:field{alias2:field2}}\"}";

            var actual = queryGenerator.GetQuery<string>(); // Typeparameter is ignored since it just returns the fields

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
                            arguments: null
                        ),
                        new GraphQLField(
                            alias: "alias3",
                            field: "field3",
                            fields: null,
                            arguments: null
                        ),
                    },
                    arguments: null),
                new GraphQLField(
                    alias: "alias4",
                    field: "field4",
                    fields: null,
                    arguments: null
                )
            };
            var fieldBuilder = new FieldBuilderMock(fields);
            var queryGenerator = new GraphQLQueryGenerator(fieldBuilder);
            var expected = "{\"query\":\"query{alias:field{alias2:field2 alias3:field3} alias4:field4}\"}";

            var actual = queryGenerator.GetQuery<string>(); // Typeparameter is ignored since it just returns the fields

            Assert.Equal(expected, actual);
        }
    }
}
