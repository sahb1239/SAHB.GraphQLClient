using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.GraphQLArguments
{
    public class GraphQLArgumentsAttributeTest
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public GraphQLArgumentsAttributeTest()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Test_Single_Usage_Argument()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest1)).ToList();

            // Check if single field is found
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest1.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument1" && argument.VariableName == "variable1"));
        }

        public class QueryToTest1
        {
            [GraphQLArguments("argument1", "String", "variable1")]
            public string Field1 { get; set; }
        }

        [Fact]
        public void Test_Multiple_Usage_Argument()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest2)).ToList();

            // Check if fields is found
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest2.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument1" && argument.VariableName == "variable1"));
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest2.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument2" && argument.VariableName == "variable2"));
        }

        public class QueryToTest2
        {
            [GraphQLArguments("argument1", "String", "variable1")]
            [GraphQLArguments("argument2", "String", "variable2")]
            public string Field1 { get; set; }
        }

        [Fact]
        public void Test_Multiple_Usage_ClassArgument_Argument()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest3)).ToList<GraphQLField>();

            // Check if fields is found
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest3.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument1" && argument.VariableName == "variable1"));
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest3.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument2" && argument.VariableName == "variable2"));
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest3.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument3" && argument.VariableName == "variable3"));
        }

        public class QueryToTest3
        {
            [GraphQLArguments("argument1", "String", "variable1")]
            [GraphQLArguments("argument2", "String", "variable2")]
            public SubQueryToTest3 Field1 { get; set; }
        }

        [GraphQLArguments("argument3", "String", "variable3")]
        public class SubQueryToTest3
        {

        }

        [Fact]
        public void Test_Inline_Argument_True()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest4)).ToList();

            // Check if single field is found
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest4.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument1" && argument.VariableName == "variable1" &&
                             argument.InlineArgument == true));
        }

        public class QueryToTest4
        {
            [GraphQLArguments("argument1", "String", "variable1", false, true)]
            public string Field1 { get; set; }
        }

        [Fact]
        public void Test_Inline_Argument_Required_True()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest5)).ToList();

            // Check if single field is found
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest5.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument1" && argument.VariableName == "variable1" &&
                             argument.IsRequired == true));
        }

        public class QueryToTest5
        {
            [GraphQLArguments("argument1", "String", "variable1", true)]
            public string Field1 { get; set; }
        }

        [Fact]
        public void Test_Inline_Argument_Required_False()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest6)).ToList();

            // Check if single field is found
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest6.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument1" && argument.VariableName == "variable1" &&
                             argument.IsRequired == false));
        }

        public class QueryToTest6
        {
            [GraphQLArguments("argument1", "String", "variable1", false)]
            public string Field1 { get; set; }
        }

        [Fact]
        public void Test_Inline_Argument_Required_Default_False()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest7)).ToList();

            // Check if single field is found
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest7.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument1" && argument.VariableName == "variable1" &&
                             argument.IsRequired == false));
        }

        public class QueryToTest7
        {
            [GraphQLArguments("argument1", "String", "variable1")]
            public string Field1 { get; set; }
        }

        [Fact]
        public void Test_Default_Value()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(DefaultValueQuery)).ToList();

            // Check if single field is found
            Assert.Contains(fields,
                field => field.Alias == nameof(DefaultValueQuery.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument1" && argument.VariableName == "variable1" &&
                             (string)argument.DefaultValue == "SomeDefaultValue"));
        }

        public class DefaultValueQuery
        {
            [GraphQLArguments("argument1", "String", "variable1", isRequired: false, inlineArgument: true, defaultValue: "SomeDefaultValue")]
            public string Field1 { get; set; }
        }
    }
}
