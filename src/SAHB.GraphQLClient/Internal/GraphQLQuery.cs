using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQL.Client.Deserialization;
using SAHB.GraphQL.Client.FieldBuilder;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.QueryGenerator;

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
        private readonly IGraphQLOperation _operation;
        private readonly string _url;
        private readonly GraphQLQueryArgument[] _arguments;

        public GraphQLQuery(IGraphQLOperation operation, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod, string authorizationToken, string authorizationMethod, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLHttpExecutor executor, IGraphQLDeserialization deserilization)
        {
            this._operation = operation;
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _httpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            this._deserilization = deserilization;
            _authorizationMethod = authorizationMethod;
            this._queryGenerator = queryGenerator;
            _authorizationToken = authorizationToken;
            _arguments = arguments;
        }

        /// <inheritdoc />
        public async Task<T> Execute()
        {
            // Generate query
            var query = _queryGenerator.GenerateQuery(_operation, _arguments);

            // Get result
            var result = await _executor.ExecuteQuery(query, _url, _httpMethod, _authorizationToken, _authorizationMethod).ConfigureAwait(false);

            // Deserilize
            var deserilizationResult = _deserilization.DeserializeResult<T>(result, _operation.SelectionSet);

            if (deserilizationResult?.Errors?.Any() ?? false)
                throw new GraphQLErrorException(query: query, errors: deserilizationResult.Errors);

            return deserilizationResult?.Data;
        }
    }
}
