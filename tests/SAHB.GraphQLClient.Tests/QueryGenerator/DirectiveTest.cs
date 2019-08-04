using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator
{
    public class DirectiveTest
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;

        public DirectiveTest()
        {
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        [Fact]
        public void QueryGenerator_Generates_Directive()
        {
            // Arrange
            var field =
                new GraphQLField(alias: null, field: "field1", fields: null, type: typeof(string), targetTypes: null, arguments: new List<GraphQLFieldArguments>() { }, directives: new List<GraphQLFieldDirective>
                {
                    new GraphQLFieldDirective("if", new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("if", "Boolean", "ifvariable", isRequired: true, inlineArgument: true, defaultValue: "DefaultValue")
                    })
                });

            var expected = "{\"query\":\"query{field1 @if(if:\\\"DefaultValue\\\")}\"}";

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, new[] { field });

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void QueryGenerator_Generates_Directive_Variable()
        {
            // Arrange
            var field =
                new GraphQLField(alias: null, field: "field1", fields: null, type: typeof(string), targetTypes: null, arguments: new List<GraphQLFieldArguments>() { }, directives: new List<GraphQLFieldDirective>
                {
                    new GraphQLFieldDirective("if", new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("if", "Boolean", "ifvariable", isRequired: true, inlineArgument: false, defaultValue: "DefaultValue")
                    })
                });

            var expected = "{\"query\":\"query($ifvariable:Boolean){field1 @if(if:$ifvariable)}\",\"variables\":{\"ifvariable\":\"DefaultValue\"}}";

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, new[] { field });

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void QueryGenerator_Should_Throw_If_Required_Argument_Is_Not_Filled()
        {
            // Arrange
            var field =
                new GraphQLField(alias: null, field: "field1", fields: null, type: typeof(string), targetTypes: null, arguments: new List<GraphQLFieldArguments>() { }, directives: new List<GraphQLFieldDirective>
                {
                    new GraphQLFieldDirective("if", new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("if", "Boolean", "ifvariable", isRequired: true, inlineArgument: true)
                    })
                });

            // Act / Assert
            var exception = Assert.Throws<GraphQLArgumentsRequiredException>(() =>
                _queryGenerator.GenerateQuery(GraphQLOperationType.Query, new[] { field }));

            Assert.Single(exception.Arguments);
            Assert.Equal("ifvariable", exception.Arguments.First().VariableName);
            Assert.Equal("if", exception.Arguments.First().ArgumentName);
        }
    }
}
