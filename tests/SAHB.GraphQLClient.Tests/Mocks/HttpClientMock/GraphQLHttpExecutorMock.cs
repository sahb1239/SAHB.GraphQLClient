using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.Result;
using Xunit;
using System.Net.Http.Headers;

namespace SAHB.GraphQLClient.Tests.GraphQLClient.HttpClientMock
{
    public class GraphQLHttpExecutorMock : IGraphQLHttpExecutor
    {
        private readonly string _requiredQuery;
        private readonly HttpResponseHeaders _requiredHeaders;
        private readonly string _response;

        public GraphQLHttpExecutorMock(string response, string requiredQuery)
        {
            _requiredQuery = requiredQuery;
            _response = response;
        }

        public GraphQLHttpExecutorMock(string response, string requiredQuery, HttpResponseHeaders requiredHeaders) : this(response, requiredQuery)
        {
            _requiredHeaders = requiredHeaders;
        }

        public HttpRequestMessage LastRequest { get; private set; }

        public Task<GraphQLDataResult<T>> ExecuteQuery<T>(string query, string url, HttpMethod method, string authorizationToken = null,
            string authorizationMethod = "Bearer") where T : class
        {
            // Check if query is correct
            Assert.Equal(_requiredQuery, query);

            var result = JsonConvert.DeserializeObject<GraphQLDataResult<T>>(_response);
            result.Headers = _requiredHeaders;

            return Task.FromResult(result);
        }
    }
}
