using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAHB.GraphQL.Client.Internal;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Internal;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Batching.Internal
{
    // ReSharper disable once InconsistentNaming
    internal class GraphQLBatchMerger
    {
        private readonly GraphQLOperationType _graphQLOperationType;
        private readonly string _url;
        private readonly HttpMethod _httpMethod;
        private readonly IDictionary<string, string> _headers;
        private readonly string _authorizationToken;
        private readonly string _authorizationMethod;
        private readonly IGraphQLHttpExecutor _executor;
        private readonly IGraphQLFieldBuilder _fieldBuilder;
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        private readonly IGraphQLDeserialization _graphQLDeserialization;
        private readonly IDictionary<string, IEnumerable<GraphQLFieldWithOverridedAlias>> _fields;
        private readonly IDictionary<string, GraphQLQueryArgumentWithOverriddenVariable[]> _arguments;
        private int _identifierCount = 0;
        private bool _isExecuted = false;
        private GraphQLDataResult<JObject> _result;
        private string _executedQuery;

        public GraphQLBatchMerger(GraphQLOperationType graphQLOperationType, string url, HttpMethod httpMethod, IDictionary<string, string> headers, string authorizationToken, string authorizationMethod, IGraphQLHttpExecutor executor, IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLDeserialization graphQLDeserialization)
        {
            _graphQLOperationType = graphQLOperationType;
            _url = url;
            _httpMethod = httpMethod;
            _headers = headers;
            _authorizationToken = authorizationToken;
            _authorizationMethod = authorizationMethod;
            _executor = executor;
            _fieldBuilder = fieldBuilder;
            _queryGenerator = queryGenerator;
            _graphQLDeserialization = graphQLDeserialization;
            _fields = new Dictionary<string, IEnumerable<GraphQLFieldWithOverridedAlias>>();
            _arguments = new Dictionary<string, GraphQLQueryArgumentWithOverriddenVariable[]>();
        }

        public IGraphQLQuery<T> AddQuery<T>(params GraphQLQueryArgument[] arguments)
            where T : class
        {
            if (_isExecuted)
                throw new GraphQLBatchAlreadyExecutedException();

            var identifier = $"batch{_identifierCount++}";

            // Get fields
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(T)).Select(field =>
                new GraphQLFieldWithOverridedAlias(string.IsNullOrWhiteSpace(field.Alias) ? field.Field : field.Alias,
                    field)).ToList();

            // Add fields
            _fields.Add(identifier, fields);
            _arguments.Add(identifier, arguments
                .Select(e => new GraphQLQueryArgumentWithOverriddenVariable(e))
                .ToArray());

            return new GraphQLBatchQuery<T>(this, identifier);
        }

        public Task<T> GetValue<T>(string identifier) 
            where T : class
        {
            return GetDeserializedResult<T>(identifier);
        }

        public async Task<GraphQLDataResult<T>> GetDetailedValue<T>(string identifier)
            where T : class
        {
            var deserialized = await GetDeserializedResult<T>(identifier);

            return new GraphQLDataResult<T>
            {
                Data = deserialized,
                Headers = _result.Headers
            };
        }

        public async Task Execute()
        {
            if (_isExecuted)
                throw new GraphQLBatchAlreadyExecutedException();

            _isExecuted = true;

            // Update fields so they don't conflict
            UpdateAlias();

            // Update arguments so they don't conflict
            UpdateArguments();

            // Get all fields
            var fields = _fields.SelectMany(e => e.Value).ToList();

            // Generate query
            _executedQuery = _queryGenerator.GenerateQuery(_graphQLOperationType, fields,
                _arguments.SelectMany(e => e.Value).ToArray());

            // Execute query
            var serverResult = await _executor.ExecuteQuery(query: _executedQuery, url: _url, method: _httpMethod, authorizationToken: _authorizationToken, authorizationMethod: _authorizationMethod, headers: _headers).ConfigureAwait(false);

            // Deserilize result
            _result = _graphQLDeserialization.DeserializeResult<JObject>(serverResult.Response, fields);

            // Set headers
            _result.Headers = serverResult.Headers;
        }

        private void UpdateAlias()
        {
            // Update fields
            foreach (var fieldsWithIdentifier in _fields)
            {
                foreach (var field in fieldsWithIdentifier.Value)
                {
                    field.Alias = fieldsWithIdentifier.Key + "_" + field.Alias;
                }
            }
        }

        private void UpdateArguments()
        {
            // Update arguments
            foreach (var fieldsWithIdentifier in _fields)
            {
                foreach (var fieldArguments in Helper.GetAllArgumentsFromFields(fieldsWithIdentifier.Value))
                {
                    foreach (var argument in fieldArguments.Value)
                    {
                        argument.VariableName = fieldsWithIdentifier.Key + "_" + argument.VariableName;
                    }
                }
            }

            // Update received arguments
            foreach (var argumentsWithIdentitfier in _arguments)
            {
                foreach (var argument in argumentsWithIdentitfier.Value)
                {
                    argument.VariableName = argumentsWithIdentitfier.Key + "_" +  argument.VariableName;
                }
            }
        }

        private async Task<T> GetDeserializedResult<T>(string identifier)
            where T : class
        {
            if (!_isExecuted)
                await Execute().ConfigureAwait(false);

            if (_result.ContainsErrors)
            {
                throw new GraphQLErrorException(query: _executedQuery, errors: _result.Errors);
            }

            // Create new JObject
            JObject deserilizeFrom = new JObject();

            // Get all fields
            foreach (var field in _fields[identifier])
            {
                // Add field with previous alias to JObject
                deserilizeFrom.Add(field.Inner.Alias, _result.Data[field.Alias]);
            }

            // Deserialize from
            return _graphQLDeserialization.DeserializeResult<T>(jsonObject: deserilizeFrom, fields: _fields[identifier]);
        }

        public bool Executed => _isExecuted;

        // ReSharper disable once InconsistentNaming
        /// <inheritdoc />
        private class GraphQLFieldWithOverridedAlias : GraphQLField
        {
            public GraphQLFieldWithOverridedAlias(string alias, GraphQLField field)
                : base(alias, field: field.Field, fields: field.SelectionSet,
                    arguments: field.Arguments, type: field.BaseType, targetTypes: field.TargetTypes)
            {
                Inner = field;
            }

            public GraphQLField Inner { get; }
        }
    }
}