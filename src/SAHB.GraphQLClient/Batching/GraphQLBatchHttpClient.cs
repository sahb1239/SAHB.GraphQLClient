using System.Net.Http;
using SAHB.GraphQLClient.Batching.Internal;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryBuilder;

namespace SAHB.GraphQLClient.Batching
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    public class GraphQLBatchHttpClient : IGraphQLBatchHttpClient
    {
        private readonly IGraphQLHttpExecutor _executor;
        private readonly IGraphQLFieldBuilder _fieldBuilder;
        private readonly IGraphQLQueryBuilderFromFields _queryBuilder;

        /// <summary>
        /// Initilizes a new instance of a GraphQLBatchHttpClient which supports generating batched queries
        /// </summary>
        /// <param name="executor">The <see cref="IGraphQLHttpExecutor"/> used to execute the GraphQL request</param>
        /// <param name="fieldBuilder">The <see cref="IGraphQLFieldBuilder"/> used for generating the fields used for generating the query</param>
        /// <param name="queryBuilder">The <see cref="IGraphQLQueryBuilder"/> used for generating the queries</param>
        public GraphQLBatchHttpClient(IGraphQLHttpExecutor executor, IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryBuilderFromFields queryBuilder)
        {
            _executor = executor;
            _fieldBuilder = fieldBuilder;
            _queryBuilder = queryBuilder;
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
    }
}