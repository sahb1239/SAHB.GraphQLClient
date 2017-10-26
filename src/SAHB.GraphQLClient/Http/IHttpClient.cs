using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Http
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> request);
    }
}