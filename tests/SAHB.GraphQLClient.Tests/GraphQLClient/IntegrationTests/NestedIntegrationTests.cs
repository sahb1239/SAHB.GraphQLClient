using System.Threading.Tasks;
using Newtonsoft.Json;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Extentions;
using Xunit;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Executor;
using FakeItEasy;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using SAHB.GraphQLClient.Filtering;

namespace SAHB.GraphQLClient.Tests.GraphQLClient.IntegrationTests
{
    public class NestedIntegrationTests
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        private readonly IGraphQLFieldBuilder _fieldBuilder;
        private readonly IGraphQLDeserialization _deserilization;

        public NestedIntegrationTests()
        {
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
            _fieldBuilder = new GraphQLFieldBuilder();
            _deserilization = new GraphQLDeserilization();
        }

        [Fact]
        public async Task TestGraphQLClient()
        {
            var responseContent = "{\"data\":{\"Me\":{\"Firstname\":\"Søren\", Age:\"24\", \"lastname\": \"Bjergmark\"}}}";

            var httpClientMock = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            A.CallTo(() => httpClientMock.ExecuteQuery("{\"query\":\"query{me{firstname age lastname}}\"}",
                A<string>.Ignored,
                A<HttpMethod>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<IDictionary<string, string>>.Ignored,
                A<CancellationToken>.Ignored))
                .Returns(new GraphQLExecutorResponse
                {
                    Response = responseContent
                });

            var client = new GraphQLHttpClient(httpClientMock, _fieldBuilder, _queryGenerator, _deserilization, new QueryGeneratorFilter());

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
