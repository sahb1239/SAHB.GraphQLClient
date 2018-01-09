using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryBuilder;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Batching.Internal
{
    // ReSharper disable once InconsistentNaming
    internal class GraphQLBatchMerger
    {
        private readonly string _url;
        private readonly HttpMethod _httpMethod;
        private readonly string _authorizationToken;
        private readonly string _authorizationMethod;
        private readonly IGraphQLHttpExecutor _executor;
        private readonly IGraphQLFieldBuilder _fieldBuilder;
        private readonly IGraphQLQueryBuilderFromFields _queryBuilder;
        private readonly IDictionary<string, IEnumerable<GraphQLFieldWithOverridedAlias>> _fields;
        private readonly IDictionary<string, GraphQLQueryArgument[]> _arguments;
        private int _identifierCount = 0;
        private bool _isExecuted = false;
        private GraphQLDataResult<JObject> _result;

        public GraphQLBatchMerger(string url, HttpMethod httpMethod, string authorizationToken, string authorizationMethod, IGraphQLHttpExecutor executor, IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryBuilderFromFields queryBuilder)
        {
            _url = url;
            _httpMethod = httpMethod;
            _authorizationToken = authorizationToken;
            _authorizationMethod = authorizationMethod;
            _executor = executor;
            _fieldBuilder = fieldBuilder;
            _queryBuilder = queryBuilder;
            _fields = new Dictionary<string, IEnumerable<GraphQLFieldWithOverridedAlias>>();
            _arguments = new Dictionary<string, GraphQLQueryArgument[]>();
        }

        public IGraphQLQuery<T> AddQuery<T>(params GraphQLQueryArgument[] arguments)
            where T : class
        {
            if (_isExecuted)
                throw new NotSupportedException("Cannot add query to already executed batch");

            var identifier = $"{_identifierCount++}";

            // Add fields
            _fields.Add(identifier,
                _fieldBuilder.GetFields(typeof(T)).Select(field =>
                    new GraphQLFieldWithOverridedAlias(identifier + "_" + field.Alias, field)));
            _arguments.Add(identifier, arguments);

            return new GraphQLBatchQuery<T>(this, identifier);
        }

        public async Task<T> GetValue<T>(string identitifer) 
            where T : class
        {
            if (!_isExecuted)
                await Execute();
            
            // Create new JObject
            JObject deserilizeFrom = new JObject();

            // Get all fields
            foreach (var field in _fields[identitifer])
            {
                // Add field with previous alias to JObject
                deserilizeFrom.Add(field.Inner.Alias, _result.Data[field.Alias]);
            }

            // Deserilize from
            return deserilizeFrom.ToObject<T>();
        }

        public async Task Execute()
        {
            if (_isExecuted)
                throw new NotSupportedException("Batch is already executed");

            _isExecuted = true;

            // Check if all variable names for arguments which contains dublicates
            if (_arguments.SelectMany(e => e.Value).GroupBy(e => e.VariableName).Any(e => e.Count() > 1))
            {
                UpdateArguments();
            }

            // Generate query
            var query = _queryBuilder.GetQuery(_fields.SelectMany(e => e.Value),
                _arguments.SelectMany(e => e.Value).ToArray());

            // Execute query
            _result =
                await _executor.ExecuteQuery<JObject>(query, _url, _httpMethod, _authorizationToken, _authorizationMethod);
        }

        private void UpdateArguments()
        {
            throw new NotImplementedException("Arguments with same variable names is not supported at the moment");
        }

        public bool Executed => _isExecuted;

        // ReSharper disable once InconsistentNaming
        /// <inheritdoc />
        private class GraphQLFieldWithOverridedAlias : GraphQLField
        {
            public GraphQLFieldWithOverridedAlias(string alias, GraphQLField field)
                : base(alias, field: field.Field, fields: field.Fields,
                    arguments: field.Arguments, type: field.Type,
                    propertyInfo: field.PropertyInfo)
            {
                Inner = field;
            }

            public GraphQLField Inner { get; }
        }
    }
}