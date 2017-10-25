using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Http;
using SAHB.GraphQLClient.QueryBuilder;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    public class GraphQLClient : IGraphQLClient
    {
        private readonly IHttpClient _client;
        private readonly IGraphQLQueryBuilder _queryBuilder;

        public GraphQLClient(IHttpClient client, IGraphQLQueryBuilder queryBuilder)
        {
            _client = client;
            _queryBuilder = queryBuilder;
        }

        public Task<T> Get<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            return Query<T>(url, authorizationToken, authorizationMethod, arguments);
        }

        public Task<T> Query<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            return Query<T>(url, HttpMethod.Post, authorizationToken, authorizationMethod, arguments);
        }

        public Task<T> Mutate<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            return Mutate<T>(url, HttpMethod.Post, authorizationToken, authorizationMethod, arguments);
        }

        public Task<T> Get<T>(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            return Query<T>(url, httpMethod, authorizationToken, authorizationMethod, arguments);
        }

        public async Task<T> Query<T>(string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            var result = await ExecuteQuery<T>(_queryBuilder.GetQuery<T>(arguments), url, httpMethod, authorizationToken, authorizationMethod);
            return result.Data;
        }

        public async Task<T> Mutate<T>(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            var result = await ExecuteQuery<T>(_queryBuilder.GetMutation<T>(arguments), url, httpMethod, authorizationToken, authorizationMethod);
            return result.Data;
        }

        private async Task<GraphQLDataResult<T>> ExecuteQuery<T>(string query, string url, HttpMethod httpMethod, string authorizationToken, string authorizationMethod) where T : class
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (httpMethod == null) throw new ArgumentNullException(nameof(httpMethod));

            // Send request
            HttpResponseMessage response = await _client.SendItemAsync(httpMethod, url, query, authorizationToken, authorizationMethod);
            response.EnsureSuccessStatusCode();

            // Deserilize response
            string stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GraphQLDataResult<T>>(stringResponse);
        }
    }
}