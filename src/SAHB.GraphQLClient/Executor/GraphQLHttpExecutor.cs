using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Executor
{
    // ReSharper disable once InconsistentNaming
    public class GraphQLHttpExecutor : IGraphQLHttpExecutor
    {
        private readonly HttpClient _client;

        public GraphQLHttpExecutor()
        {
            // Add httpClient
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        public async Task<GraphQLDataResult<T>> ExecuteQuery<T>(string query, string url, HttpMethod method, string authorizationToken = null, string authorizationMethod = "Bearer") where T : class
        {
            // Check parameters for null
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (method == null) throw new ArgumentNullException(nameof(method));

            // Initilizes request message
            var requestMessage = new HttpRequestMessage(method, url)
            {
                Content = new StringContent(query, Encoding.UTF8, "application/json")
            };

            // Add authorization info
            if (authorizationToken != null)
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue(authorizationMethod, authorizationToken);
            }

            // Send request
            HttpResponseMessage response = await _client.SendAsync(requestMessage);

            // Ensure success response
            response.EnsureSuccessStatusCode();

            // Deserilize response
            string stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GraphQLDataResult<T>>(stringResponse);
        }
    }
}
