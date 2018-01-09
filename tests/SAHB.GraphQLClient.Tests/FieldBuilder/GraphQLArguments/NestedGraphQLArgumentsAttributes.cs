using System.Collections.Generic;
using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.GraphQLArguments
{
    public class NestedGraphQLArgumentsAttributes
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public NestedGraphQLArgumentsAttributes()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Test_GraphQLSkipArgument_In_Fieds()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(SkipQueryToTest1)).ToList<GraphQLField>();

            // Check if any argument is found
            Assert.Contains(fields,
                field => field.Alias == nameof(SkipQueryToTest1.SkipField1) && field.Arguments.Count() == 1);
        }

        [Fact]
        public void Test_GraphQLSkipArgument_Correct_ArgumentName_And_Type()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(SkipQueryToTest1)).ToList<GraphQLField>();

            // Check if the argument is found
            Assert.Contains(fields,
                field => field.Alias == nameof(SkipQueryToTest1.SkipField1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "skip" && argument.ArgumentType == "Int" &&
                             argument.VariableName == "variableskip"));
        }

        public class SkipQueryToTest1
        {
            [GraphQLSkipArguments("variableskip")]
            public IEnumerable<string> SkipField1 { get; set; }
        }

        [Fact]
        public void Test_GraphQLTakeArgument_In_Fieds()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(TakeQueryToTest1)).ToList<GraphQLField>();

            // Check if any argument is found
            Assert.Contains(fields,
                field => field.Alias == nameof(TakeQueryToTest1.TakeField1) && field.Arguments.Count() == 1);
        }

        [Fact]
        public void Test_GraphQLTakeArgument_Correct_ArgumentName_And_Type()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(TakeQueryToTest1)).ToList<GraphQLField>();

            // Check if the argument is found
            Assert.Contains(fields,
                field => field.Alias == nameof(TakeQueryToTest1.TakeField1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "take" && argument.ArgumentType == "Int" &&
                             argument.VariableName == "variabletake"));
        }

        public class TakeQueryToTest1
        {
            [GraphQLTakeArguments("variabletake")]
            public IEnumerable<string> TakeField1 { get; set; }
        }

        [Fact]
        public void Test_GraphQLSkipTakeArgument_Correct_ArgumentName_And_Type()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(SkipTakeQueryToTest1)).ToList<GraphQLField>();

            // Check if the argument is found
            Assert.Contains(fields,
                field => field.Alias == nameof(SkipTakeQueryToTest1.SkipTakeField1) && 
                        field.Arguments.Any(argument =>
                             argument.ArgumentName == "skip" && argument.ArgumentType == "Int" &&
                             argument.VariableName == "variableskip") && 
                        field.Arguments.Any(argument =>
                             argument.ArgumentName == "take" && argument.ArgumentType == "Int" &&
                             argument.VariableName == "variabletake"));
        }

        public class SkipTakeQueryToTest1
        {
            [GraphQLSkipArguments("variableskip")]
            [GraphQLTakeArguments("variabletake")]
            public IEnumerable<string> SkipTakeField1 { get; set; }
        }
    }
}
