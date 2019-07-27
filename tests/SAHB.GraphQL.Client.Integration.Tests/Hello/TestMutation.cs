using GraphQL.Types;
using SAHB.GraphQLClient.Integration.Tests.TestServer;
using SAHB.GraphQLClient;
using System.Threading.Tasks;
using Xunit;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Integration.Tests
{
    public class TestMutation : IClassFixture<GraphQLWebApplicationFactory<TestMutation.TestSchema>>
    {
        private readonly GraphQLWebApplicationFactory<TestSchema> _factory;

        public TestMutation(GraphQLWebApplicationFactory<TestSchema> factory)
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

        public class TestSchema : Schema
        {
            public TestSchema()
            {
                Mutation = new TestSchemaMutation();
            }
        }

        private class TestSchemaMutation : ObjectGraphType
        {
            public TestSchemaMutation()
            {
                Field<StringGraphType>("hello", resolve: context => "mutation");
            }
        }

        private class TestHelloMutation
        {
            public string Hello { get; set; }
        }
    }
}
