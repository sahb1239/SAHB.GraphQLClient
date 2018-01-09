using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Batching;
using SAHB.GraphQLClient.Batching.Internal;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Internal;
using SAHB.GraphQLClient.QueryBuilder;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    public class GraphQLHttpClient : IGraphQLHttpClient
    {
        private readonly IGraphQLHttpExecutor _executor;
        private readonly IGraphQLFieldBuilder _fieldBuilder;
        private readonly IGraphQLQueryBuilderFromFields _queryBuilder;

        /// <summary>
        /// Initilizes a new instance of GraphQL client which supports generating GraphQL queries and mutations from a <see cref="Type"/>
        /// </summary>
        /// <param name="executor">The <see cref="IGraphQLHttpExecutor"/> to use for the GraphQL client</param>
        /// <param name="fieldBuilder">The <see cref="IGraphQLFieldBuilder"/> used for generating the fields used for generating the query</param>
        /// <param name="queryBuilder">The <see cref="IGraphQLQueryBuilderFromFields"/> used for the GraphQL client</param>
        public GraphQLHttpClient(IGraphQLHttpExecutor executor, IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryBuilderFromFields queryBuilder)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _fieldBuilder = fieldBuilder ?? throw new ArgumentNullException(nameof(fieldBuilder));
            _queryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));
        }

        /// <summary>
        /// Initilizes a new instance of GraphQL client which supports generating GraphQL queries and mutations from a <see cref="Type"/> using the default <see cref="IGraphQLHttpExecutor"/> and the default <see cref="IGraphQLQueryBuilder"/>
        /// </summary>
        /// <returns>A new instance of the GraphQL client</returns>
        public static IGraphQLHttpClient Default()
        {
            return new GraphQLHttpClient(new GraphQLHttpExecutor(), new GraphQLFieldBuilder(), new GraphQLQueryBuilderFromFields());
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
        public Task<T> Query<T>(string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            var query = CreateQuery<T>(url, httpMethod, authorizationToken, authorizationMethod, arguments);
            return query.Execute();
        }

        /// <inheritdoc />
        public Task<T> Mutate<T>(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            var query = CreateMutation<T>(url, httpMethod, authorizationToken, authorizationMethod, arguments);
            return query.Execute();
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> CreateQuery<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            return CreateQuery<T>(url, HttpMethod.Post, authorizationToken, authorizationMethod, arguments);
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> CreateMutation<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            return CreateMutation<T>(url, HttpMethod.Post, authorizationToken, authorizationMethod, arguments);
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> CreateMutation<T>(string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            var query = _queryBuilder.GetMutation(_fieldBuilder.GetFields(typeof(T)), arguments);
            return GetGraphQLQuery<T>(query, url, httpMethod, authorizationToken, authorizationMethod, arguments);
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> CreateQuery<T>(string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            var query = _queryBuilder.GetQuery(_fieldBuilder.GetFields(typeof(T)), arguments);
            return GetGraphQLQuery<T>(query, url, httpMethod, authorizationToken, authorizationMethod, arguments);
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
            return new GraphQLBatch(url, httpMethod, authorizationToken, authorizationMethod, _executor, _fieldBuilder, _queryBuilder);
        }

        private IGraphQLQuery<T> GetGraphQLQuery<T>(string query, string url, HttpMethod httpMethod,
            string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            return new GraphQLQuery<T>(query, url, httpMethod, authorizationToken, authorizationMethod, _executor);
        }
    }
}