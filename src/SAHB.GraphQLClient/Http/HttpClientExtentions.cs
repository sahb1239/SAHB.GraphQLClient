using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Http
{
    [Obsolete]
    public static class HttpClientExtentions
    {
        [Obsolete]
        public static Task<HttpResponseMessage> SendAsync<T>(this IHttpClient client, HttpMethod method, string url, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            return client.SendAsync(() =>
            {
                var requestMessage = new HttpRequestMessage(method, url);

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                return requestMessage;
            });
        }

        public static Task<HttpResponseMessage> SendItemAsync(this IHttpClient client, HttpMethod method, string url, string item,
            string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            return client.SendAsync(() =>
            {
                var requestMessage = new HttpRequestMessage(method, url)
                {
                    Content = new StringContent(item, System.Text.Encoding.UTF8, "application/json")
                };

                if (authorizationToken != null)
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
                }

                return requestMessage;
            });
        }
    }
}