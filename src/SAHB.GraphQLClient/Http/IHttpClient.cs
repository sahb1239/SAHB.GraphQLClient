using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Http
{
    [Obsolete]
    public interface IHttpClient
    {
        Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> request);
    }
}