using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Http
{
    public class HttpClient : IHttpClient
    {
        private readonly System.Net.Http.HttpClient _client;

        public HttpClient()
        {
            _client = new System.Net.Http.HttpClient();
            _client.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        public Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> request)
        {
            return _client.SendAsync(request());
        }
    }
}