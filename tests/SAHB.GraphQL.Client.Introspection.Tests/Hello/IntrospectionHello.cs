using SAHB.GraphQL.Client.Testserver.Tests.Schemas.Hello;
using SAHB.GraphQL.Client.TestServer;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQLClient.Introspection.Tests
{
    public class IntrospectionHello : IClassFixture<GraphQLWebApplicationFactory<HelloQuerySchema>>
    {
        private readonly GraphQLWebApplicationFactory<HelloQuerySchema> _factory;

        public IntrospectionHello(GraphQLWebApplicationFactory<HelloQuerySchema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test_IntrospectionResult_Does_Not_Contain_Errors()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var result = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").ExecuteDetailed();

            // Assert
            Assert.False(result.ContainsErrors);
        }
    }
}
