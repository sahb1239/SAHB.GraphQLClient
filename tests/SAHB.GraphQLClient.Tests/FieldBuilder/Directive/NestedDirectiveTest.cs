using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using System.Linq;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.Directive
{
    public class NestedDirectiveTest
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public NestedDirectiveTest()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Has_Directive()
        {
            // Arrange / Act
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(HelloWithDirectiveQuery)).ToList();

            // Assert
            var helloField = fields.First().SelectionSet.First();
            Assert.Equal(nameof(HelloWithDirective.Hello), helloField.Alias);

            Assert.Single(helloField.Directives);
            Assert.Equal("include", helloField.Directives.First().DirectiveName);
            Assert.Empty(helloField.Directives.First().Arguments);
        }

        [Fact]
        public void Has_Directive_Argument()
        {
            // Arrange / Act
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(HelloWithDirectiveArgumentQuery)).ToList();

            // Assert
            var helloField = fields.First().SelectionSet.First();
            Assert.Equal(nameof(HelloWithDirective.Hello), helloField.Alias);

            Assert.Single(helloField.Directives);
            Assert.Equal("include", helloField.Directives.First().DirectiveName);

            Assert.Single(helloField.Directives.First().Arguments);
            Assert.Equal("if", helloField.Directives.First().Arguments.First().ArgumentName);
            Assert.Equal("Boolean", helloField.Directives.First().Arguments.First().ArgumentType);
            Assert.Equal("variableif", helloField.Directives.First().Arguments.First().VariableName);
        }

        public class HelloWithDirectiveQuery
        {
            public HelloWithDirective Nested { get; set; }
        }

        public class HelloWithDirectiveArgumentQuery
        {
            public HelloWithDirectiveArgument Nested { get; set; }
        }

        public class HelloWithDirective
        {
            [GraphQLDirective("include")]
            public string Hello { get; set; }
        }

        public class HelloWithDirectiveArgument
        {
            [GraphQLDirective("include")]
            [GraphQLDirectiveArgument("include", "if", "Boolean", "variableif")]
            public string Hello { get; set; }
        }
    }
}
