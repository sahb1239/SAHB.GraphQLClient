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
using System.Threading;

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
        private readonly IDictionary<string, string> _headers;
        private readonly Func<GraphQLField, bool> _filter = null;

        [Obsolete]
        public GraphQLQuery(GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod, string authorizationToken, string authorizationMethod, IDictionary<string, string> headers, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLHttpExecutor executor, IGraphQLDeserialization deserilization)
            : this(operationType: operationType,
                  selectionSet: selectionSet,
                  arguments: arguments,
                  url: url,
                  httpMethod: httpMethod,
                  filter: null,
                  authorizationToken: authorizationToken,
                  authorizationMethod: authorizationMethod,
                  headers: headers,
                  queryGenerator: queryGenerator,
                  executor: executor,
                  deserilization: deserilization)
        {
        }

        public GraphQLQuery(GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod, Func<GraphQLField, bool> filter, string authorizationToken, string authorizationMethod, IDictionary<string, string> headers, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLHttpExecutor executor, IGraphQLDeserialization deserilization)
        {
            this._url = url ?? throw new ArgumentNullException(nameof(url));
            this._httpMethod = httpMethod;
            this._executor = executor ?? throw new ArgumentNullException(nameof(executor));
            this._deserilization = deserilization;
            this._authorizationMethod = authorizationMethod;
            this._queryGenerator = queryGenerator;
            this._authorizationToken = authorizationToken;
            this.OperationType = operationType;
            this.SelectionSet = selectionSet;
            this._arguments = arguments;
            this._headers = headers;
            this._filter = filter;
        }

        private GraphQLOperationType OperationType { get; }
        private IEnumerable<GraphQLField> SelectionSet { get; }

        /// <inheritdoc />
        public async Task<T> Execute(CancellationToken cancellationToken = default)
        {
            var result = await GetDataResult(cancellationToken).ConfigureAwait(false);
            return result?.Data;
        }

        public async Task<GraphQLDataResult<T>> ExecuteDetailed(CancellationToken cancellationToken = default)
        {
            var result = await GetDataResult(cancellationToken).ConfigureAwait(false);
            return new GraphQLDataResult<T>
            {
                Data = result.Data,
                Headers = result.Headers
            };
        }

        private async Task<GraphQLDataResult<T>> GetDataResult(CancellationToken cancellationToken)
        {
            // Generate query
            var query = _queryGenerator.GenerateQuery(OperationType, SelectionSet, _filter, _arguments);

            // Get result
            var result = await _executor.ExecuteQuery(
                query: query,
                url: _url,
                method: _httpMethod,
                authorizationToken: _authorizationToken,
                authorizationMethod: _authorizationMethod,
                cancellationToken: cancellationToken,
                headers: _headers).ConfigureAwait(false);

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
        [Obsolete]
        public GraphQLQuery(GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod, string authorizationToken, string authorizationMethod, IDictionary<string, string> headers, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLHttpExecutor executor, IGraphQLDeserialization deserilization) : base(operationType, selectionSet, arguments, url, httpMethod, authorizationToken, authorizationMethod, headers, queryGenerator, executor, deserilization)
        {
        }

        public GraphQLQuery(GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod, Func<GraphQLField, bool> filter, string authorizationToken, string authorizationMethod, IDictionary<string, string> headers, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLHttpExecutor executor, IGraphQLDeserialization deserilization) : base(operationType, selectionSet, arguments, url, httpMethod, filter, authorizationToken, authorizationMethod, headers, queryGenerator, executor, deserilization)
        {
        }
    }
}
