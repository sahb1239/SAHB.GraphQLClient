using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.QueryGenerator
{
    public class GraphQLQueryArgumentFieldTests
    {
        [Fact]
        public void Gets_Correct_Field_With_One_Level()
        {
            // Arrange / Act
            var argument = new GraphQLQueryArgument<QueryClass>("variableName", "argumentValue", 
                expr => expr.Field);

            // Assert
            Assert.Equal("Field", argument.Field);
        }

        [Fact]
        public void Gets_Correct_Field_With_Nested_One_Level()
        {
            // Arrange / Act
            var argument = new GraphQLQueryArgument<QueryClass>("variableName", "argumentValue",
                expr => expr.NestedField.Field);

            // Assert
            Assert.Equal("NestedField.Field", argument.Field);
        }

        [Fact]
        public void Gets_Correct_Field_With_Nested_Two_Level()
        {
            // Arrange / Act
            var argument = new GraphQLQueryArgument<QueryClass>("variableName", "argumentValue",
                expr => expr.NestedField.NestedField);

            // Assert
            Assert.Equal("NestedField.NestedField", argument.Field);
        }

        public class QueryClass
        {
            public string Field { get; set; }
            public QueryClass NestedField { get; set; }
        }
    }
}
