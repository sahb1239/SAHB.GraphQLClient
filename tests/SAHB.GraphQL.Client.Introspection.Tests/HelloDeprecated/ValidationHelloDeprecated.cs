using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQL.Client.Testserver.Tests.Schemas.HelloDeprecated;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using SAHB.GraphQLClient.QueryGenerator;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Introspection.Tests.Hello
{
    public class ValidationHelloDeprecated : IClassFixture<GraphQLWebApplicationFactory<HelloDeprecatedQuerySchema>>
    {
        private readonly GraphQLWebApplicationFactory<HelloDeprecatedQuerySchema> _factory;

        public ValidationHelloDeprecated(GraphQLWebApplicationFactory<HelloDeprecatedQuerySchema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Validate_Hello_Query_IsDeprecated()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>(
                "http://localhost/graphql", 
                arguments: new GraphQLQueryArgument("fieldsIncludeDeprecated", true))
                .Execute();
            var validationOutput = introspectionQuery.ValidateGraphQLType<TestHelloQuery>(GraphQLOperationType.Query);

            // Assert
            Assert.Single(validationOutput);
            Assert.Equal(ValidationType.Field_Deprecated, validationOutput.First().ValidationType);
        }

        private class TestHelloQuery
        {
            public string Hello { get; set; }
        }
    }
}
