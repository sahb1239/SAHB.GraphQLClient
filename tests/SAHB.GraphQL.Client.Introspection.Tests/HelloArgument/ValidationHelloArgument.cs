using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQL.Client.Testserver.Tests.Schemas.Hello;
using SAHB.GraphQL.Client.Testserver.Tests.Schemas.HelloArgument;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.Introspection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Introspection.Tests.HelloArgument
{
    public class ValidationHelloArgument : IClassFixture<GraphQLWebApplicationFactory<HelloArgumentQuerySchema>>
    {
        private readonly GraphQLWebApplicationFactory<HelloArgumentQuerySchema> _factory;

        public ValidationHelloArgument(GraphQLWebApplicationFactory<HelloArgumentQuerySchema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Validate_Hello_Query_IsValid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
            var validationOutput = introspectionQuery.ValidateGraphQLType<TestHelloQuery>(GraphQLOperationType.Query);

            // Assert
            Assert.Empty(validationOutput);
        }

        [Fact]
        public async Task Validate_Hello_Query_Argument_NotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
            var validationOutput = introspectionQuery.ValidateGraphQLType<TestNotImplementedArgument>(GraphQLOperationType.Query);

            // Assert
            Assert.Single(validationOutput);
            Assert.Equal(ValidationType.Argument_Not_Found, validationOutput.First().ValidationType);
        }

        [Fact]
        public async Task Validate_Hello_Query_Argument_WrongType()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
            var validationOutput = introspectionQuery.ValidateGraphQLType<TestWrongTypeArgument>(GraphQLOperationType.Query);

            // Assert
            Assert.Single(validationOutput);
            Assert.Equal(ValidationType.Argument_Invalid_Type, validationOutput.First().ValidationType);
        }

        private class TestHelloQuery
        {
            [GraphQLArguments("argument1", "String", "argument1")]
            [GraphQLArguments("argument2", "Int", "argument2")]
            [GraphQLArguments("argument3", "Id", "argument3")]
            public string Hello { get; set; }
        }

        private class TestNotImplementedArgument
        {
            [GraphQLArguments("otherArgument", "Id", "otherArgument")]
            public string Hello { get; set; }
        }

        private class TestWrongTypeArgument
        {
            [GraphQLArguments("argument1", "Id", "argument1")]
            public string Hello { get; set; }
        }
    }
}
