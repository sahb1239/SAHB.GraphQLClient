using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SAHB.GraphQL.Client;
using SAHB.GraphQL.Client.Deserialization;
using SAHB.GraphQLClient.Batching;
using SAHB.GraphQLClient.Batching.Internal;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.Builder.Internal;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Internal;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient
{
    /// <inheritdoc />
    public class GraphQLHttpClient : IGraphQLHttpClient
    {
        public IGraphQLFieldBuilder FieldBuilder { get; }

        public IGraphQLHttpExecutor HttpExecutor { get; }
        public IGraphQLQueryGeneratorFromFields QueryGenerator { get; }

        private IGraphQLDeserialization Deserialization { get; }

        public GraphQLHttpClient(IGraphQLHttpExecutor httpExecutor, IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLDeserialization deserialization)
        {
            HttpExecutor = httpExecutor ?? throw new ArgumentNullException(nameof(httpExecutor));
            FieldBuilder = fieldBuilder ?? throw new ArgumentNullException(nameof(fieldBuilder));
            QueryGenerator = queryGenerator ?? throw new ArgumentNullException(nameof(queryGenerator));
            Deserialization = deserialization ?? throw new ArgumentNullException(nameof(deserialization));
        }

        /// <inheritdoc />
        public static GraphQLHttpClient Default()
        {
            return new GraphQLHttpClient(new GraphQLHttpExecutor(), new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
        }

        /// <inheritdoc />
        public static GraphQLHttpClient Default(HttpClient client)
        {
            return new GraphQLHttpClient(new GraphQLHttpExecutor(client), new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
        }

        /// <inheritdoc />
        public Task<T> Execute<T>(GraphQLOperationType operationType, string url = null, HttpMethod httpMethod = null, IDictionary<string, string> headers = null, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            // Generate selectionSet
            var selectionSet = FieldBuilder.GenerateSelectionSet(typeof(T));

            // Generate query
            var query = new GraphQLQueryToExecute(operationType, selectionSet, arguments);

            // Execute
            return Execute<T>(query: query, url: url, httpMethod: httpMethod, headers: headers, authorizationMethod: authorizationMethod, authorizationToken: authorizationToken);
        }

        /// <inheritdoc />
        public Task<dynamic> Execute(GraphQLOperationType operationType, Action<IGraphQLBuilder> builder, string url = null, HttpMethod httpMethod = null, IDictionary<string, string> headers = null, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments)
        {
            // Get builder
            var build = new GraphQLBuilder();
            builder(build);

            // Get the fields and query
            var selectionSet = build.GetFields();

            // Generate query
            var query = new GraphQLQueryToExecute(operationType, selectionSet, arguments);

            // Execute
            return Execute<dynamic>(query: query, url: url, httpMethod: httpMethod, headers: headers, authorizationMethod: authorizationMethod, authorizationToken: authorizationToken);
        }

        private async Task<T> Execute<T>(IGraphQLQueryToExecute query, string url, HttpMethod httpMethod, IDictionary<string, string> headers, string authorizationMethod, string authorizationToken) where T : class
        {
            var result = await ExecuteQuery<T>(query, url, httpMethod, headers, authorizationMethod, authorizationToken).ConfigureAwait(false);
            return result.Data;
        }

        private async Task<GraphQLDataResult<T>> ExecuteQuery<T>(IGraphQLQueryToExecute query, string url, HttpMethod httpMethod, IDictionary<string, string> headers, string authorizationMethod, string authorizationToken) where T : class
        {
            // Generate query
            var requestQuery = QueryGenerator.GenerateQuery(query.OperationType, query.SelectionSet, query.Arguments.ToArray());
            
            // Get response
            string stringResponse = await HttpExecutor.ExecuteQuery(requestQuery, url, httpMethod, headers: headers, authorizationToken: authorizationToken, authorizationMethod: authorizationMethod).ConfigureAwait(false);

            // Deserilize
            var result = Deserialization.DeserializeResult<T>(stringResponse, query.SelectionSet);
            if (result?.Errors?.Any() ?? false)
                throw new GraphQLErrorException(query: requestQuery, errors: result.Errors);

            return result;
        }

        #region Old methods
        /// <inheritdoc />
        public IGraphQLQuery<dynamic> CreateQuery(Action<IGraphQLBuilder> builder, string url, string authorizationToken = null,
             string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments)
        {
            return CreateQuery(builder, url, HttpMethod.Post, authorizationToken, authorizationMethod, arguments);
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> CreateQuery<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            return CreateQuery<T>(url, HttpMethod.Post, authorizationToken, authorizationMethod, arguments);
        }

        /// <inheritdoc />
        public IGraphQLQuery<dynamic> CreateMutation(Action<IGraphQLBuilder> builder, string url, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments)
        {
            return CreateMutation(builder, url, HttpMethod.Post, authorizationToken, authorizationMethod, arguments);
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> CreateMutation<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            return CreateMutation<T>(url, HttpMethod.Post, authorizationToken, authorizationMethod, arguments);
        }

        /// <inheritdoc />
        public IGraphQLQuery<dynamic> CreateMutation(Action<IGraphQLBuilder> builder, string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments)
        {
            var build = new GraphQLBuilder();
            builder(build);

            // Get the fields and query
            var selectionSet = build.GetFields();

            // Get query
            return GetGraphQLQuery(GraphQLOperationType.Mutation, selectionSet, arguments, url, httpMethod, authorizationToken, authorizationMethod);
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> CreateMutation<T>(string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            var selectionSet = FieldBuilder.GenerateSelectionSet(typeof(T));
            return GetGraphQLQuery<T>(GraphQLOperationType.Mutation, selectionSet, arguments, url, httpMethod, authorizationToken, authorizationMethod);
        }

        /// <inheritdoc />
        public IGraphQLQuery<dynamic> CreateQuery(Action<IGraphQLBuilder> builder, string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments)
        {
            var build = new GraphQLBuilder();
            builder(build);

            // Get the fields and query
            var selectionSet = build.GetFields();

            // Get query
            return GetGraphQLQuery(GraphQLOperationType.Query, selectionSet, arguments, url, httpMethod, authorizationToken, authorizationMethod);
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> CreateQuery<T>(string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            var selectionSet = FieldBuilder.GenerateSelectionSet(typeof(T));
            return GetGraphQLQuery<T>(GraphQLOperationType.Query, selectionSet, arguments, url, httpMethod, authorizationToken, authorizationMethod);
        }

        /// <inheritdoc />
        public IGraphQLBatch CreateBatch(string url, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            return CreateBatch(url, HttpMethod.Post, authorizationToken, authorizationMethod);
        }

        /// <inheritdoc />
        public IGraphQLBatch CreateBatch(string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            return new GraphQLBatch(url, httpMethod, authorizationToken, authorizationMethod, HttpExecutor, FieldBuilder, QueryGenerator, Deserialization);
        }

        // ReSharper disable once InconsistentNaming
        private IGraphQLQuery<dynamic> GetGraphQLQuery(GraphQLOperationType operationType, IEnumerable<IGraphQLField> selectionSet, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod,
            string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            return new GraphQLQuery<dynamic>(operationType, selectionSet, arguments, url, httpMethod, authorizationToken, authorizationMethod, QueryGenerator, HttpExecutor, Deserialization);
        }

        // ReSharper disable once InconsistentNaming
        private IGraphQLQuery<T> GetGraphQLQuery<T>(GraphQLOperationType operationType, IEnumerable<IGraphQLField> selectionSet, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod,
            string authorizationToken = null,
            string authorizationMethod = "Bearer") where T : class
        {
            return new GraphQLQuery<T>(operationType, selectionSet, arguments, url, httpMethod, authorizationToken, authorizationMethod, QueryGenerator, HttpExecutor, Deserialization);
        }

        #endregion

        #region Logging

        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// Contains a logger factory for the GraphQLHttpClient
        /// </summary>
        public ILoggerFactory LoggerFactory
        {
            internal get { return _loggerFactory; }
            set
            {
                _loggerFactory = value;
                if (_loggerFactory != null)
                {
                    Logger = _loggerFactory.CreateLogger<GraphQLHttpClient>();
                }
            }
        }

        /// <summary>
        /// Contains the logger for the class
        /// </summary>
        private ILogger<GraphQLHttpClient> Logger { get; set; }

        #endregion
    }
}