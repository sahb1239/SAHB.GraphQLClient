using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.NonNested
{
    public class NonNestedArgumentTest
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public NonNestedArgumentTest()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Test_Single_Usage_Argument()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest)).ToList();

            // Check if single field is found
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "min1" && argument.VariableName == "var1"));
        }

        [Fact]
        public void Test_Multiple_Usage_Argument()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest)).ToList();

            // Check if fields is found
            Assert.Contains(fields,
                field => 
                field.Alias == nameof(QueryToTest.Field2) && 
                field.Arguments.Any(argument => argument.ArgumentName == "min2" && argument.VariableName == "var2") &&
                field.Arguments.Any(argument => argument.ArgumentName == "max2" && argument.VariableName == "var3"));
        }

        [Fact]
        public void Test_Multiple_Same_Name_Usage()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest)).ToList();

            // Check if fields is found
            Assert.Contains(fields,
                field =>
                    field.Alias == nameof(QueryToTest.Field3) &&
                    field.Arguments.Any(argument => argument.ArgumentName == "min" && argument.VariableName == "var4") &&
                    field.Arguments.Any(argument => argument.ArgumentName == "min" && argument.VariableName == "var5") &&
                    field.Arguments.Any(argument => argument.ArgumentName == "max" && argument.VariableName == "var6"));
        }

        [Fact]
        public void Test_Correct_Number_Of_Arguments()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest)).ToList();

            // Check if correct number of arguments
            Assert.Equal(1, fields.Where(e => e.Alias == nameof(QueryToTest.Field1)).SelectMany(e => e.Arguments).Count());
            Assert.Equal(2, fields.Where(e => e.Alias == nameof(QueryToTest.Field2)).SelectMany(e => e.Arguments).Count());
            Assert.Equal(3, fields.Where(e => e.Alias == nameof(QueryToTest.Field3)).SelectMany(e => e.Arguments).Count());
        }

        public class QueryToTest
        {
            [GraphQLArguments("min1", "String", "var1")]
            public string Field1 { get; set; }

            [GraphQLArguments("min2", "String", "var2")]
            [GraphQLArguments("max2", "String", "var3")]
            public string Field2 { get; set; }

            [GraphQLArguments("min", "String", "var4")]
            [GraphQLArguments("min", "String", "var5")]
            [GraphQLArguments("max", "String", "var6")]
            public string Field3 { get; set; }
        }
    }
}
