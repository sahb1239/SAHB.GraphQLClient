using System.Net.Http;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Batching.Internal
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    internal class GraphQLBatch : IGraphQLBatch
    {
        private readonly GraphQLBatchMerger _batch;

        internal GraphQLBatch(string url, HttpMethod httpMethod, string authorizationToken, string authorizationMethod, IGraphQLHttpExecutor executor, IGraphQLFieldBuilder fieldBuilder, IGraphQLQueryGeneratorFromFields queryGenerator)
        {
            _batch = new GraphQLBatchMerger(url, httpMethod, authorizationToken, authorizationMethod, executor, fieldBuilder, queryGenerator);
        }

        /// <inheritdoc />
        public IGraphQLQuery<T> Query<T>(params GraphQLQueryArgument[] arguments) where T : class
        {
            if (_batch.Executed)
            {
                throw new GraphQLBatchAlreadyExecutedException();
            }

            return _batch.AddQuery<T>(arguments);
        }

        /// <inheritdoc />
        public bool IsExecuted()
        {
            return _batch.Executed;
        }
    }
}
