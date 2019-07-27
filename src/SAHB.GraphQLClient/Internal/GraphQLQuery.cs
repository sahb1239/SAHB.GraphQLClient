using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Executor;

namespace SAHB.GraphQLClient.Internal
{
    using Result;

    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    internal class GraphQLQuery : IGraphQLQuery
    {
        private readonly string _authorizationToken;
        private readonly string _authorizationMethod;
        private readonly IGraphQLHttpExecutor _executor;
        private readonly HttpMethod _httpMethod;
        private readonly string _url;
        private readonly string _query;

        public GraphQLQuery(string query, string url, HttpMethod httpMethod, string authorizationToken, string authorizationMethod, IGraphQLHttpExecutor executor)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _httpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _authorizationMethod = authorizationMethod;
            _authorizationToken = authorizationToken;
        }

        /// <inheritdoc />
        public async Task<dynamic> Execute()
        {
            var result = await GetDataResult();
            return result?.Data;
        }

        public async Task<GraphQLDataDetailedResult<dynamic>> ExecuteDetailed()
        {
            var result = await GetDataResult();
            return new GraphQLDataDetailedResult<dynamic>
            {
                Data = result.Data,
                Headers = result.Headers
            };
        }

        private async Task<GraphQLDataResult<dynamic>> GetDataResult()
        {
            var result = await _executor.ExecuteQuery<dynamic>(_query, _url, _httpMethod, _authorizationToken, _authorizationMethod).ConfigureAwait(false);
            if (result?.Errors?.Any() ?? false)
                throw new GraphQLErrorException(query: _query, errors: result.Errors);

            return result;
        }
    }

    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    internal class GraphQLQuery<T> : IGraphQLQuery<T> where T : class
    {
        private readonly string _authorizationToken;
        private readonly string _authorizationMethod;
        private readonly IGraphQLHttpExecutor _executor;
        private readonly HttpMethod _httpMethod;
        private readonly string _url;
        private readonly string _query;

        public GraphQLQuery(string query, string url, HttpMethod httpMethod, string authorizationToken, string authorizationMethod, IGraphQLHttpExecutor executor)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query));
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _httpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _authorizationMethod = authorizationMethod;
            _authorizationToken = authorizationToken;
        }

        /// <inheritdoc />
        public async Task<T> Execute()
        {
            var result = await GetDataResult();
            return result?.Data;
        }

        public async Task<GraphQLDataDetailedResult<T>> ExecuteDetailed()
        {
            var result = await GetDataResult();
            return new GraphQLDataDetailedResult<T>
            {
                Data = result.Data,
                Headers = result.Headers
            };
        }

        private async Task<GraphQLDataResult<T>> GetDataResult()
        {
            var result = await _executor.ExecuteQuery<T>(_query, _url, _httpMethod, _authorizationToken, _authorizationMethod).ConfigureAwait(false);
            if (result?.Errors?.Any() ?? false)
                throw new GraphQLErrorException(query: _query, errors: result.Errors);

            return result;
        }
    }
}
