using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.Result;
using Xunit;

namespace SAHB.GraphQLClient.Tests.GraphQLClient.HttpClientMock
{
    using System.Net.Http.Headers;

    public class GraphQLHttpExecutorMock : IGraphQLHttpExecutor
    {
        private readonly string _requiredQuery;
        private readonly HttpResponseHeaders _requiredHeaders;
        private readonly string _response;

        public GraphQLHttpExecutorMock(string response, string requiredQuery, HttpResponseHeaders requiredHeaders)
        {
            _requiredQuery = requiredQuery;
            _requiredHeaders = requiredHeaders;
            _response = response;
        }

        public GraphQLHttpExecutorMock(string response, string requiredQuery) : this(response, requiredQuery, null)
        {

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
