using GraphQL.Types;
using SAHB.GraphQL.Client.Integration.Tests.TestServer;
using SAHB.GraphQLClient;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Integration.Tests
{
    public class TestQuery : IClassFixture<GraphQLWebApplicationFactory<TestQuery.TestSchema>>
    {
        private readonly GraphQLWebApplicationFactory<TestSchema> _factory;

        public TestQuery(GraphQLWebApplicationFactory<TestSchema> factory)
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
            var result = await graphQLClient.CreateQuery<TestHelloQuery>("http://localhost/graphql").Execute();

            // Assert
            Assert.Equal("query", result.Hello);
        }

        public class TestSchema : Schema
        {
            public TestSchema()
            {
                Query = new TestSchemaQuery();
            }
        }

        private class TestSchemaQuery : ObjectGraphType
        {
            public TestSchemaQuery()
            {
                Field<StringGraphType>("hello", resolve: context => "query");
            }
        }

        private class TestHelloQuery
        {
            public string Hello { get; set; }
        }
    }
}
