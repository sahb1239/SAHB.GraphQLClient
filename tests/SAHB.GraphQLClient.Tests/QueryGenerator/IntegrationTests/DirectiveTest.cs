using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Extentions;
using Xunit;
using Newtonsoft.Json;

namespace SAHB.GraphQLClient.Tests.QueryGenerator.IntegrationTests
{
    public class DirectiveTest
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public DirectiveTest()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        [Fact]
        public void Has_Directive()
        {
            // Arrange 
            var expected =
               "{\"query\":\"query{hello @include}\"}";

            // Act
            var actual = _queryGenerator.GetQuery<HelloWithDirective>(_fieldBuilder);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Has_Directive_Argument(bool argumentValue)
        {
            // Arrange 
            var expected =
               "{\"query\":\"query{hello @include(if:" + JsonConvert.SerializeObject(argumentValue) + ")}\"}";

            // Act
            var actual = _queryGenerator.GetQuery<HelloWithDirectiveArgument>(_fieldBuilder, new GraphQLQueryDirectiveArgument("variableif", "include", argumentValue));

            // Assert
            Assert.Equal(expected, actual);
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
