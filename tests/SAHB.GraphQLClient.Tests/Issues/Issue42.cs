using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.Issues
{
    public class Issue42
    {
        [Fact]
        public void Test_Variable_Complex_Dynamic_Type()
        {
            // Arrange
            var expected = "{\"query\":\"query($variableName:Type!){field(argumentName:$variableName)}\",\"variables\":{\"variableName\":{\"field1\":\"value1\",\"field2\":\"value2\"}}}";
            var fieldBuilder = new GraphQLFieldBuilder();
            var queryBuilder = new GraphQLQueryGeneratorFromFields();

            // Act
            // Get fields
            var fields =
                fieldBuilder.GenerateSelectionSet(typeof(Query));
            var actual = queryBuilder.GenerateQuery(GraphQLOperationType.Query, fields,
                new GraphQLQueryArgument("variableName", new { field1 = "value1", field2 = "value2" }));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Variable_Complex_Static_Type()
        {
            // Arrange
            var expected = "{\"query\":\"query($variableName:Type!){field(argumentName:$variableName)}\",\"variables\":{\"variableName\":{\"Field1\":\"value1\",\"Field2\":\"value2\"}}}";
            var fieldBuilder = new GraphQLFieldBuilder();
            var queryBuilder = new GraphQLQueryGeneratorFromFields();

            // Act
            // Get fields
            var fields =
                fieldBuilder.GenerateSelectionSet(typeof(Query));
            var actual = queryBuilder.GenerateQuery(GraphQLOperationType.Query, fields,
                new GraphQLQueryArgument("variableName", new StaticArgument { Field1 = "value1", Field2 = "value2" }));

            // Assert
            Assert.Equal(expected, actual);
        }

        public class Query
        {
            [GraphQLArguments("argumentName", "Type!", "variableName")]
            public string Field { get; set; }
        }

        public class StaticArgument
        {
            public string Field1 { get; set; }
            public string Field2 { get; set; }
        }
    }
}
