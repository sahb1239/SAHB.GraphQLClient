using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Internal
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    internal class GraphQLQuery<T> : IGraphQLQuery<T> where T : class
    {
        private readonly string _authorizationToken;
        private readonly string _authorizationMethod;
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        private readonly IGraphQLHttpExecutor _executor;
        private readonly IGraphQLDeserialization _deserilization;
        private readonly HttpMethod _httpMethod;
        private readonly string _url;
        private readonly GraphQLQueryArgument[] _arguments;

        public GraphQLQuery(GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod, string authorizationToken, string authorizationMethod, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLHttpExecutor executor, IGraphQLDeserialization deserilization)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _httpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            this._deserilization = deserilization;
            _authorizationMethod = authorizationMethod;
            this._queryGenerator = queryGenerator;
            _authorizationToken = authorizationToken;
            OperationType = operationType;
            SelectionSet = selectionSet;
            _arguments = arguments;
        }

        private GraphQLOperationType OperationType { get; }
        private IEnumerable<GraphQLField> SelectionSet { get; }

        /// <inheritdoc />
        public async Task<T> Execute()
        {
            var result = await GetDataResult().ConfigureAwait(false);
            return result?.Data;
        }

        public async Task<GraphQLDataResult<T>> ExecuteDetailed()
        {
            var result = await GetDataResult().ConfigureAwait(false);
            return new GraphQLDataResult<T>
            {
                Data = result.Data,
                Headers = result.Headers
            };
        }

        private async Task<GraphQLDataResult<T>> GetDataResult()
        {
            // Generate query
            var query = _queryGenerator.GenerateQuery(OperationType, SelectionSet, _arguments);

            // Get result
            var result = await _executor.ExecuteQuery(query, _url, _httpMethod, _authorizationToken, _authorizationMethod).ConfigureAwait(false);

            // Deserilize
            var deserilizationResult = _deserilization.DeserializeResult<T>(result.Response, SelectionSet);

            if (deserilizationResult?.Errors?.Any() ?? false)
                throw new GraphQLErrorException(query: query, errors: deserilizationResult.Errors);

            // Set headers
            deserilizationResult.Headers = result.Headers;

            return deserilizationResult;
        }
    }

    internal class GraphQLQuery : GraphQLQuery<dynamic>, IGraphQLQuery
    {
        public GraphQLQuery(GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod, string authorizationToken, string authorizationMethod, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLHttpExecutor executor, IGraphQLDeserialization deserilization) : base(operationType, selectionSet, arguments, url, httpMethod, authorizationToken, authorizationMethod, queryGenerator, executor, deserilization)
        {
        }
    }
}
