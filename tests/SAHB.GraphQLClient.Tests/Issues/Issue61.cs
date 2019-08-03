using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.Issues
{
    public class Issue61
    {
        [Fact]
        public void GraphQLQueryGeneratorFromFields_Should_Throw_When_No_Matching_Argument_Found()
        {
            // Arrange
            var fieldGenerator = new GraphQLFieldBuilder();
            var queryGenerator = new GraphQLQueryGeneratorFromFields();
            
            // Act / Assert
            var exception = Assert.Throws<GraphQLArgumentVariableNotFoundException>(() =>
            {
                // Get the query
                var query = queryGenerator.GenerateQuery(GraphQLOperationType.Query, fieldGenerator.GenerateSelectionSet(typeof(SampleQuery)),
                    new GraphQLQueryArgument("helloArgument", "Value"));
            });

            // Assert
            // Should contain one element
            Assert.Equal(1, exception.Arguments.Count());

            // Argument should contain helloArgument and value
            Assert.Equal("helloArgument", exception.Arguments.First().VariableName);
            Assert.Equal("Value", exception.Arguments.First().ArgumentValue);
        }

        [Fact]
        public void GraphQLQueryGeneratorFromFields_Should_Throw_When_No_Matching_Argument_Found_Two_Arguments()
        {
            // Arrange
            var fieldGenerator = new GraphQLFieldBuilder();
            var queryGenerator = new GraphQLQueryGeneratorFromFields();

            var argument1 = new GraphQLQueryArgument("hello1Argument", "Value1");
            var argument2 = new GraphQLQueryArgument("hello2Argument", "Value2");

            // Act / Assert
            var exception = Assert.Throws<GraphQLArgumentVariableNotFoundException>(() =>
            {
                // Get the query
                var query = queryGenerator.GenerateQuery(GraphQLOperationType.Query, fieldGenerator.GenerateSelectionSet(typeof(SampleQuery)),
                    argument1, argument2);
            });

            // Assert
            // Should contain two element
            Assert.Equal(2, exception.Arguments.Count());

            // Argument should contain helloArgument and value
            Assert.Contains(argument1, exception.Arguments);
            Assert.Contains(argument2, exception.Arguments);
        }


        [Fact]
        public async Task Client_Should_Throw_When_No_Matching_Argument_Found()
        {
            // Arrange
            var client = GraphQLHttpClient.Default();

            // Act / Assert
            var exception = await Assert.ThrowsAsync<GraphQLArgumentVariableNotFoundException>(() =>
            {
                // Get the query
                var query = client.CreateQuery<SampleQuery>("url", arguments:
                    new GraphQLQueryArgument("helloVariableName", "helloVariableValue")).Execute();
                return query;
            });

            // Assert
            // Should contain one element
            Assert.Equal(1, exception.Arguments.Count());

            // Argument should contain helloArgument and value
            Assert.Equal("helloVariableName", exception.Arguments.First().VariableName);
            Assert.Equal("helloVariableValue", exception.Arguments.First().ArgumentValue);
        }


        public class SampleQuery
        {
            public string Hello { get; set; }
        }

        [Fact]
        public void GraphQLQueryGeneratorFromFields_Should_Throw_When_No_Matching_Argument_Found_And_Only_One_Of_2_Arguments_Found()
        {
            // Arrange
            var fieldGenerator = new GraphQLFieldBuilder();
            var queryGenerator = new GraphQLQueryGeneratorFromFields();

            // Act / Assert
            var exception = Assert.Throws<GraphQLArgumentVariableNotFoundException>(() =>
            {
                // Get the query
                var query = queryGenerator.GenerateQuery(GraphQLOperationType.Query, fieldGenerator.GenerateSelectionSet(typeof(SampleQuery2)),
                    new GraphQLQueryArgument("hello1Argument", "Value1"), new GraphQLQueryArgument("hello2Argument", "Value2"));
            });

            // Assert
            // Should contain one element
            Assert.Equal(1, exception.Arguments.Count());

            // Argument should contain helloArgument and value
            Assert.Equal("hello2Argument", exception.Arguments.First().VariableName);
            Assert.Equal("Value2", exception.Arguments.First().ArgumentValue);
        }

        public class SampleQuery2
        {
            [GraphQLArguments("hello1Argument", "String", "hello1Argument")]
            public string Hello1 { get; set; }
            public string Hello2 { get; set; }
        }
    }
}
