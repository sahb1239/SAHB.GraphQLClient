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
    public class GraphQLHttpExecutorMock : IGraphQLHttpExecutor
    {
        private readonly string _requiredQuery;
        private readonly string _response;

        public GraphQLHttpExecutorMock(string response, string requiredQuery)
        {
            _requiredQuery = requiredQuery;
            _response = response;
        }

        public HttpRequestMessage LastRequest { get; private set; }

        public Task<GraphQLDataResult<T>> ExecuteQuery<T>(string query, string url, HttpMethod method, string authorizationToken = null,
            string authorizationMethod = "Bearer") where T : class
        {
            // Check if query is correct
            Assert.Equal(_requiredQuery, query);

            return Task.FromResult(JsonConvert.DeserializeObject<GraphQLDataResult<T>>(_response));
        }
    }
}
