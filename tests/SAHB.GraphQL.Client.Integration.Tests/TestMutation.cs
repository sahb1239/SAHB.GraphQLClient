using GraphQL.Types;
using SAHB.GraphQLClient.Integration.Tests.TestServer;
using SAHB.GraphQLClient;
using System.Threading.Tasks;
using Xunit;

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
