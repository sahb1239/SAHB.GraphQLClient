using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using SAHB.GraphQL.Client.Deserialization;
using SAHB.GraphQL.Client.FieldBuilder;
using SAHB.GraphQLClient.Batching;
using SAHB.GraphQLClient.Batching.Internal;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.Builder.Internal;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Internal;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    public class GraphQLHttpClient : IGraphQLHttpClient
    {
        private readonly IGraphQLHttpExecutor _executor;
        private readonly IGraphQLFieldBuilder _fieldBuilder;
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        private readonly IGraphQLDeserialization _deserilization;
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

        /// <summary>
        /// Initializes a new instance of GraphQL client which supports generating GraphQL queries and mutations from a <see cref="Type"/>
        /// </summary>
        /// <param name="executor">The <see cref="IGraphQLHttpExecutor"/> to use for the GraphQL client</param>
        /// <param name="fieldBuilder">The <see cref="IGraphQLFieldBuilder"/> used for generating the fields used for generating the query</param>
        /// <param name="queryGenerator">The <see cref="IGraphQLQueryGeneratorFromFields"/> used for the GraphQL client</param>
        public GraphQLHttpClient(IGraphQLHttpExecutor executor, IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLDeserialization deserilization)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _fieldBuilder = fieldBuilder ?? throw new ArgumentNullException(nameof(fieldBuilder));
            _queryGenerator = queryGenerator ?? throw new ArgumentNullException(nameof(queryGenerator));
            _deserilization = deserilization ?? throw new ArgumentNullException(nameof(deserilization));
        }

        /// <summary>
        /// Initializes a new instance of GraphQL client which supports generating GraphQL queries and mutations from a <see cref="Type"/> using the default <see cref="IGraphQLHttpExecutor"/> and the default <see cref="IGraphQLQueryGeneratorFromFields"/>
        /// </summary>
        /// <returns>A new instance of the GraphQL client</returns>
        public static IGraphQLHttpClient Default()
        {
            return new GraphQLHttpClient(new GraphQLHttpExecutor(), new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());
        }

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

        public IGraphQLQuery<dynamic> CreateMutation(Action<IGraphQLBuilder> builder, string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments)
        {
            var build = new GraphQLBuilder();
            builder(build);

            // Get the fields and query
            var fields = build.GetFields();
            var operation = new GraphQLOperation(GraphQLOperationType.Mutation, fields);

            // Get query
            return GetGraphQLQuery(operation, arguments, url, httpMethod, authorizationToken, authorizationMethod);
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> CreateMutation<T>(string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            var operation = _fieldBuilder.GenerateOperation(typeof(T), GraphQLOperationType.Mutation);
            return GetGraphQLQuery<T>(operation, arguments, url, httpMethod, authorizationToken, authorizationMethod);
        }

        public IGraphQLQuery<dynamic> CreateQuery(Action<IGraphQLBuilder> builder, string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments)
        {
            var build = new GraphQLBuilder();
            builder(build);

            // Get the fields and query
            var fields = build.GetFields();
            var operation = new GraphQLOperation(GraphQLOperationType.Query, fields);

            // Get query
            return GetGraphQLQuery(operation, arguments, url, httpMethod, authorizationToken, authorizationMethod);
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> CreateQuery<T>(string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            var operation = _fieldBuilder.GenerateOperation(typeof(T), GraphQLOperationType.Query);
            return GetGraphQLQuery<T>(operation, arguments, url, httpMethod, authorizationToken, authorizationMethod);
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
            return new GraphQLBatch(url, httpMethod, authorizationToken, authorizationMethod, _executor, _fieldBuilder, _queryGenerator, _deserilization);
        }

        // ReSharper disable once InconsistentNaming
        private IGraphQLQuery<dynamic> GetGraphQLQuery(IGraphQLOperation operation, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod,
            string authorizationToken = null,
            string authorizationMethod = "Bearer")
        {
            return new GraphQLQuery<dynamic>(operation, arguments, url, httpMethod, authorizationToken, authorizationMethod, _queryGenerator, _executor, _deserilization);
        }

        // ReSharper disable once InconsistentNaming
        private IGraphQLQuery<T> GetGraphQLQuery<T>(IGraphQLOperation operation, GraphQLQueryArgument[] arguments, string url, HttpMethod httpMethod,
            string authorizationToken = null,
            string authorizationMethod = "Bearer") where T : class
        {
            return new GraphQLQuery<T>(operation, arguments, url, httpMethod, authorizationToken, authorizationMethod, _queryGenerator, _executor, _deserilization);
        }
    }
}