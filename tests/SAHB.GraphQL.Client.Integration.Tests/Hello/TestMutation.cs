using System.Threading.Tasks;
using Xunit;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQL.Client.Testserver.Tests.Schemas.Hello;
using SAHB.GraphQL.Client.TestServer;

namespace SAHB.GraphQLClient.Integration.Tests
{
    public class TestMutation : IClassFixture<GraphQLWebApplicationFactory<HelloMutationSchema>>
    {
        private readonly GraphQLWebApplicationFactory<HelloMutationSchema> _factory;

        public TestMutation(GraphQLWebApplicationFactory<HelloMutationSchema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestSimpleMutation()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var result = await graphQLClient.CreateMutation<TestHelloMutation>("http://localhost/graphql").Execute();

            // Assert
            Assert.Equal("mutation", result.Hello);
        }

        [Fact]
        public async Task TestBatchMutation()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var batch = graphQLClient.CreateBatch(GraphQLOperationType.Mutation, "http://localhost/graphql");
            var query1 = batch.Query<TestHelloMutation>();
            var query2 = batch.Query<TestHelloMutation>();

            var result1 = await query1.Execute();
            var result2 = await query2.Execute();

            // Assert
            Assert.Equal("mutation", result1.Hello);
            Assert.Equal("mutation", result2.Hello);
        }

        private class TestHelloMutation
        {
            public string Hello { get; set; }
        }
    }
}
