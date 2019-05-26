using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SAHB.GraphQLClient.Deserialization;
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
        /// <summary>
        /// The <see cref="IGraphQLFieldBuilder"/> used
        /// </summary>
        public IGraphQLFieldBuilder FieldBuilder { get; }

        /// <summary>
        /// The <see cref="IGraphQLHttpExecutor"/> used
        /// </summary>
        public IGraphQLHttpExecutor HttpExecutor { get; }

        /// <summary>
        /// The <see cref="IGraphQLQueryGeneratorFromFields"/> used
        /// </summary>
        public IGraphQLQueryGeneratorFromFields QueryGenerator { get; }

        /// <summary>
        /// The <see cref="IGraphQLDeserialization"/> used
        /// </summary>
        private IGraphQLDeserialization Deserialization { get; }

        public GraphQLHttpClient(IGraphQLHttpExecutor httpExecutor, IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLDeserialization deserialization)
        {
            HttpExecutor = httpExecutor ?? throw new ArgumentNullException(nameof(httpExecutor));
            FieldBuilder = fieldBuilder ?? throw new ArgumentNullException(nameof(fieldBuilder));
            QueryGenerator = queryGenerator ?? throw new ArgumentNullException(nameof(queryGenerator));
            Deserialization = deserialization ?? throw new ArgumentNullException(nameof(deserialization));
        }

        /// <summary>
        /// Returns a default <see cref="GraphQLHttpClient"/> using default implementations of <see cref="IGraphQLHttpExecutor"/>, <see cref="IGraphQLFieldBuilder"/>, <see cref="IGraphQLQueryGeneratorFromFields"/> and <see cref="IGraphQLDeserialization"/>
        /// </summary>
        /// <returns>Returns a default <see cref="GraphQLHttpClient"/></returns>
        public static GraphQLHttpClient Default()
        {
            return new GraphQLHttpClient(new GraphQLHttpExecutor(), new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
        }

        /// <summary>
        /// Returns a default <see cref="GraphQLHttpClient"/> using default implementations of <see cref="IGraphQLHttpExecutor"/> (using the specified <see cref="HttpClient"/>), <see cref="IGraphQLFieldBuilder"/>, <see cref="IGraphQLQueryGeneratorFromFields"/> and <see cref="IGraphQLDeserialization"/>
        /// </summary>
        /// <param name="client">The <see cref="HttpClient"/> to use for requesting the GraphQL server</param>
        /// <returns>Returns a default <see cref="GraphQLHttpClient"/> using the specified <see cref="HttpClient"/></returns>
        public static GraphQLHttpClient Default(HttpClient client)
        {
            return new GraphQLHttpClient(new GraphQLHttpExecutor(client), new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
        }

        /// <inheritdoc />
        public Task<T> Execute<T>(GraphQLOperationType operationType, string url = null, HttpMethod httpMethod = null, IDictionary<string, string> headers = null, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            // Generate selectionSet
            var selectionSet = FieldBuilder.GenerateSelectionSet(typeof(T));

            // Execute
            return Execute<T>(operationType, selectionSet, arguments, url: url, httpMethod: httpMethod, headers: headers, authorizationMethod: authorizationMethod, authorizationToken: authorizationToken);
        }

        /// <inheritdoc />
        public Task<dynamic> Execute(GraphQLOperationType operationType, Action<IGraphQLBuilder> builder, string url = null, HttpMethod httpMethod = null, IDictionary<string, string> headers = null, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments)
        {
            // Get builder
            var build = new GraphQLBuilder();
            builder(build);

            // Get the fields and query
            var selectionSet = build.GetFields();

            // Execute
            return Execute<dynamic>(operationType, selectionSet, arguments, url: url, httpMethod: httpMethod, headers: headers, authorizationMethod: authorizationMethod, authorizationToken: authorizationToken);
        }

        /// <inheritdoc />
        public IGraphQLBatch CreateBatch(GraphQLOperationType operationType, string url = null, HttpMethod httpMethod = null, IDictionary<string, string> headers = null, string authorizationToken = null, string authorizationMethod = "Bearer")
        {
            return new GraphQLBatch(operationType, url, httpMethod, headers, authorizationToken, authorizationMethod, HttpExecutor, FieldBuilder, QueryGenerator, Deserialization);
        }

        private async Task<T> Execute<T>(GraphQLOperationType operationType, IEnumerable<IGraphQLField> selectionSet, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod, IDictionary<string, string> headers, string authorizationMethod, string authorizationToken) where T : class
        {
            var result = await ExecuteQuery<T>(operationType, selectionSet, arguments, url, httpMethod, headers, authorizationMethod, authorizationToken).ConfigureAwait(false);
            return result.Data;
        }

        private async Task<GraphQLDataResult<T>> ExecuteQuery<T>(GraphQLOperationType operationType, IEnumerable<IGraphQLField> selectionSet, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod, IDictionary<string, string> headers, string authorizationMethod, string authorizationToken) where T : class
        {
            // Generate query
            var requestQuery = QueryGenerator.GenerateQuery(operationType, selectionSet, arguments.ToArray());
            
            // Get response
            string stringResponse = await HttpExecutor.ExecuteQuery(requestQuery, url, httpMethod, headers: headers, authorizationToken: authorizationToken, authorizationMethod: authorizationMethod).ConfigureAwait(false);

            // Deserilize
            var result = Deserialization.DeserializeResult<T>(stringResponse, selectionSet);
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
            return new GraphQLBatch(GraphQLOperationType.Query, url, httpMethod, null, authorizationToken, authorizationMethod, HttpExecutor, FieldBuilder, QueryGenerator, Deserialization);
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