using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQL.Client.Testserver.Tests.Schemas.Hello;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Introspection.Tests.Hello
{
    public class ValidationIntrospection : IClassFixture<GraphQLWebApplicationFactory<HelloQuerySchema>>
    {
        private readonly GraphQLWebApplicationFactory<HelloQuerySchema> _factory;

        public ValidationIntrospection(GraphQLWebApplicationFactory<HelloQuerySchema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Validate_Introspection_IsValid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
            var validationOutput = introspectionQuery.ValidateGraphQLType<GraphQLIntrospectionQuery>(GraphQLOperationType.Query);

            // Assert
            // GraphQL has by an error set isDeprecated to a String! instead of a Boolean! and therefore this test should return one validationError
            // This is fixed in https://github.com/graphql-dotnet/graphql-dotnet/commit/abf351892dda7bc8cf3dd83aba1ce63ae1ce11fd#diff-d9710dc6d5945261ad8c70084443f0c2
            Assert.Single(validationOutput);
            Assert.Equal("__schema.types.enumValues.isDeprecated", validationOutput.FirstOrDefault().Path);
            Assert.Equal(ValidationType.Field_Invalid_Type, validationOutput.FirstOrDefault().ValidationType);
        }
    }
}
