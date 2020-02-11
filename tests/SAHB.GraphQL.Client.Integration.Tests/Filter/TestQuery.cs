using System.Threading.Tasks;
using Xunit;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQL.Client.Testserver.Tests.Schemas.Hello;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Integration.Tests.Filter
{
    public class TestQuery : IClassFixture<GraphQLWebApplicationFactory<HelloQuerySchema>>
    {
        private readonly GraphQLWebApplicationFactory<HelloQuerySchema> _factory;

        public TestQuery(GraphQLWebApplicationFactory<HelloQuerySchema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestHello1()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var result = await graphQLClient.Execute<TestHelloQuery>(GraphQLOperationType.Query, "http://localhost/graphql", filter: e => new TestHelloQuery { Hello1 = e.Hello1 });

            // Assert
            Assert.Equal("query", result.Hello1);
            Assert.Null(result.Hello2);
        }

        [Fact]
        public async Task TestHello2()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var result = await graphQLClient.Execute<TestHelloQuery>(GraphQLOperationType.Query, "http://localhost/graphql", filter: e => new TestHelloQuery { Hello1 = e.Hello2 });

            // Assert
            Assert.Equal("query", result.Hello2);
            Assert.Null(result.Hello1);
        }

        private class TestHelloQuery
        {
            [GraphQLFieldName("hello")]
            public string Hello1 { get; set; }
            [GraphQLFieldName("hello")]
            public string Hello2 { get; set; }
        }

        private class TestHelloQueryDirective
        {
            [GraphQLDirective("include")]
            [GraphQLDirectiveArgument("include", "if", "Boolean", "variableif", isRequired: true)]
            public string Hello { get; set; }
        }
    }
}
