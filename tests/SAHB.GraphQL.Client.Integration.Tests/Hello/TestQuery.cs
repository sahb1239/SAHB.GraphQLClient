using System.Threading.Tasks;
using Xunit;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQL.Client.Testserver.Tests.Schemas.Hello;
using SAHB.GraphQL.Client.TestServer;

namespace SAHB.GraphQLClient.Integration.Tests
{
    public class TestQuery : IClassFixture<GraphQLWebApplicationFactory<HelloQuerySchema>>
    {
        private readonly GraphQLWebApplicationFactory<HelloQuerySchema> _factory;

        public TestQuery(GraphQLWebApplicationFactory<HelloQuerySchema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestSimpleQuery()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var result = await graphQLClient.Execute<TestHelloQuery>(GraphQLOperationType.Query, "http://localhost/graphql");

            // Assert
            Assert.Equal("query", result.Hello);
        }

        [Fact]
        public async Task TestBatchQuery()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var batch = graphQLClient.CreateBatch(GraphQLOperationType.Query, "http://localhost/graphql");
            var query1 = batch.Query<TestHelloQuery>();
            var query2 = batch.Query<TestHelloQuery>();

            var result1 = await query1.Execute();
            var result2 = await query2.Execute();

            // Assert
            Assert.Equal("query", result1.Hello);
            Assert.Equal("query", result2.Hello);
        }

        private class TestHelloQuery
        {
            public string Hello { get; set; }
        }
    }
}
