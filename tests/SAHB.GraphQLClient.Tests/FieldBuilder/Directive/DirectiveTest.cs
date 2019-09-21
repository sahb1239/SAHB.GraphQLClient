using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using System.Linq;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.Directive
{
    public class DirectiveTest
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public DirectiveTest()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Has_Directive()
        {
            // Arrange / Act
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(HelloWithDirective)).ToList();

            // Assert
            Assert.Single(fields);

            var helloField = fields.First();
            Assert.Equal(nameof(HelloWithDirective.Hello), helloField.Alias);

            Assert.Single(helloField.Directives);
            Assert.Equal("include", helloField.Directives.First().DirectiveName);
            Assert.Empty(helloField.Directives.First().Arguments);
        }

        [Fact]
        public void Has_Directive_Argument()
        {
            // Arrange / Act
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(HelloWithDirectiveArgument)).ToList();

            // Assert
            Assert.Single(fields);

            var helloField = fields.First();
            Assert.Equal(nameof(HelloWithDirective.Hello), helloField.Alias);

            Assert.Single(helloField.Directives);
            Assert.Equal("include", helloField.Directives.First().DirectiveName);

            Assert.Single(helloField.Directives.First().Arguments);
            Assert.Equal("if", helloField.Directives.First().Arguments.First().ArgumentName);
            Assert.Equal("Boolean", helloField.Directives.First().Arguments.First().ArgumentType);
            Assert.Equal("variableif", helloField.Directives.First().Arguments.First().VariableName);
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
