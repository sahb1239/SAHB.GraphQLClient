using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Http;
using Xunit;

namespace SAHB.GraphQLClient.Tests.GraphQLClient.HttpClientMock
{
    public class HttpClientMock : IHttpClient
    {
        private readonly string _requiredRequest;
        private readonly HttpResponseMessage _response;

        public HttpClientMock(string response, string requiredRequest)
        {
            _requiredRequest = requiredRequest;
            _response = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(response)};
        }

        public HttpClientMock(HttpResponseMessage response, string requiredRequest)
        {
            _response = response;
            _requiredRequest = requiredRequest;
        }

        public async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> request)
        {
            if (LastRequest != null)
                throw new InvalidOperationException("Multiple requests to mock not supported");

            var req = request();

            Assert.Equal(_requiredRequest, await req.Content.ReadAsStringAsync());

            LastRequest = req;
            return _response;
        }

        public HttpRequestMessage LastRequest { get; private set; }
    }
}
