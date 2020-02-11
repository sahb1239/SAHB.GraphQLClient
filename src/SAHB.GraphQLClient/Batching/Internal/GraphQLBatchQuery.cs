using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Result;
using System.Threading;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient.Batching.Internal
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    internal class GraphQLBatchQuery<T> : IGraphQLQuery<T>
        where T : class
    {
        private readonly GraphQLBatchMerger _batch;
        private readonly string _identitifer;

        internal GraphQLBatchQuery(GraphQLBatchMerger batch, string identitifer)
        {
            _batch = batch;
            _identitifer = identitifer;
        }

        /// <inheritdoc />
        public Task<T> Execute(CancellationToken cancellationToken = default)
        {
            return _batch.GetValue<T>(_identitifer, cancellationToken);
        }

        public Task<GraphQLDataResult<T>> ExecuteDetailed(CancellationToken cancellationToken = default)
        {
            return _batch.GetDetailedValue<T>(_identitifer, cancellationToken);
        }
    }
}
