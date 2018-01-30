using System.Threading.Tasks;
using Newtonsoft.Json;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Extentions;
using Xunit;

namespace SAHB.GraphQLClient.Tests.GraphQLClient.IntegrationTests
{
    public class NestedIntegrationTests
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public NestedIntegrationTests()
        {
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public async Task TestGraphQLClient()
        {
            var responseContent = "{\"data\":{\"Me\":{\"Firstname\":\"Søren\", Age:\"24\", \"lastname\": \"Bjergmark\"}}}";
            var httpClient = new HttpClientMock.GraphQLHttpExecutorMock(responseContent, "{\"query\":\"query{me{firstname age lastname}}\"}");
            var client = new GraphQLHttpClient(httpClient, _fieldBuilder, _queryGenerator);

            // Act
            var response = await client.Query<QueryToTest>("");

            // Assert
            Assert.Equal("Søren", response.Me.Firstname);
            Assert.Equal("Bjergmark", response.Me.lastname);
            Assert.Equal(24u, response.Me.Age);
        }

        public class QueryToTest
        {
            public Person Me { get; set; }
        }

        public class Person
        {
            public string Firstname { get; set; }
            public uint Age { get; set; }
            public string lastname { get; set; }
        }
    }
}
