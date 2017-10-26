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
            var fields = _fieldBuilder.GetFields(typeof(QueryToTest1)).ToList<GraphQLField>();

            // Check if single field is found
            Assert.Contains(fields,
                field => field.Alias == nameof(QueryToTest1.Field1) && field.Arguments.Any(argument =>
                             argument.ArgumentName == "argument1" && argument.VariableName == "variable1"));
        }

        public class QueryToTest1
        {
            [GraphQLArguments("argument1", "variable1")]
            public string Field1 { get; set; }
        }

        [Fact]
        public void Test_Multiple_Usage_Argument()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(QueryToTest2)).ToList<GraphQLField>();

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
            [GraphQLArguments("argument1", "variable1")]
            [GraphQLArguments("argument2", "variable2")]
            public string Field1 { get; set; }
        }

        [Fact]
        public void Test_Multiple_Usage_ClassArgument_Argument()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(QueryToTest3)).ToList<GraphQLField>();

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
            [GraphQLArguments("argument1", "variable1")]
            [GraphQLArguments("argument2", "variable2")]
            public SubQueryToTest3 Field1 { get; set; }
        }

        [GraphQLArguments("argument3", "variable3")]
        public class SubQueryToTest3
        {
            
        }
    }
}
