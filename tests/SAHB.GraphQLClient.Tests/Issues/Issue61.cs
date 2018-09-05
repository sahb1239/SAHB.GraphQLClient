using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.Issues
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
                var query = queryGenerator.GetQuery(fieldGenerator.GetFields(typeof(SampleQuery)),
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
        public void Client_Should_Throw_When_No_Matching_Argument_Found()
        {
            // Arrange
            var client = GraphQLHttpClient.Default();

            // Act / Assert
            var exception = Assert.Throws<GraphQLArgumentVariableNotFoundException>(() =>
            {
                // Get the query
                var query = client.CreateQuery< SampleQuery>("url", arguments:
                    new GraphQLQueryArgument("helloVariableName", "helloVariableValue"));
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
    }
}
