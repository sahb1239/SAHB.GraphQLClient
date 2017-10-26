using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.QueryBuilder;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    public class GraphQLHttpClient : IGraphQLHttpClient
    {
        private readonly IGraphQLHttpExecutor _executor;
        private readonly IGraphQLQueryBuilder _queryBuilder;

        /// <summary>
        /// Initilizes a new instance of GraphQL client which supports generating GraphQL queries and mutations from a <see cref="Type"/>
        /// </summary>
        /// <param name="executor">The <see cref="IGraphQLHttpExecutor"/> to use for the GraphQL client</param>
        /// <param name="queryBuilder">The <see cref="IGraphQLQueryBuilder"/> used for the GraphQL client</param>
        public GraphQLHttpClient(IGraphQLHttpExecutor executor, IGraphQLQueryBuilder queryBuilder)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _queryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));
        }

        /// <inheritdoc />
        public Task<T> Query<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            return Query<T>(url, HttpMethod.Post, authorizationToken, authorizationMethod, arguments);
        }

        /// <inheritdoc />
        public Task<T> Mutate<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            return Mutate<T>(url, HttpMethod.Post, authorizationToken, authorizationMethod, arguments);
        }

        /// <inheritdoc />
        public async Task<T> Query<T>(string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            var result = await ExecuteQuery<T>(_queryBuilder.GetQuery<T>(arguments), url, httpMethod, authorizationToken, authorizationMethod);
            if (result?.Errors?.Any() ?? false)
                throw new GraphQLErrorException(result.Errors);

            return result.Data;
        }

        /// <inheritdoc />
        public async Task<T> Mutate<T>(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            var result = await ExecuteQuery<T>(_queryBuilder.GetMutation<T>(arguments), url, httpMethod, authorizationToken, authorizationMethod);
            if (result?.Errors?.Any() ?? false)
                throw new GraphQLErrorException(result.Errors);

            return result.Data;
        }

        private Task<GraphQLDataResult<T>> ExecuteQuery<T>(string query, string url, HttpMethod httpMethod, string authorizationToken, string authorizationMethod) where T : class
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (httpMethod == null) throw new ArgumentNullException(nameof(httpMethod));

            return _executor.ExecuteQuery<T>(query, url, httpMethod, authorizationToken, authorizationMethod);
        }
    }
}