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

        public HttpClient Client => throw new NotImplementedException();

        public HttpMethod DefaultMethod { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<string> ExecuteQuery(string query, string url = null, HttpMethod method = null, string authorizationToken = null, string authorizationMethod = "Bearer", IDictionary<string, string> headers = null)
        {
            // Check if query is correct
            Assert.Equal(_requiredQuery, query);

            return Task.FromResult(_response);
        }
    }
}
