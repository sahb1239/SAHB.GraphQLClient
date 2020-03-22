using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Execution
{
    public interface IGraphQLExecutor
    {
        /// <summary>
        /// The <see cref="IGraphQLQueryGeneratorFromFields"/> used
        /// </summary>
        IGraphQLQueryGeneratorFromFields QueryGenerator { get; }

        /// <summary>
        /// The <see cref="IGraphQLHttpExecutor"/> used
        /// </summary>
        IGraphQLHttpExecutor HttpExecutor { get; }

        /// <summary>
        /// The <see cref="IGraphQLSubscriptionExecutor"/> used
        /// </summary>
        IGraphQLSubscriptionExecutor SubscriptionExecutor { get; }

        /// <summary>
        /// The <see cref="IGraphQLDeserialization"/> used
        /// </summary>
        IGraphQLDeserialization Deserialization { get; }

        Task<IGraphQLHttpResponse<TInput, TOutput, IGraphQLHttpRequest<TInput>>> ExecuteHttp<TInput, TOutput>(
            IGraphQLHttpRequest<TInput> request,
            Func<GraphQLField, bool> queryFilter,
            Expression<Func<TInput, TOutput>> outputSelector, 
            CancellationToken cancellationToken) 
            where TInput : class
            where TOutput : class;

        Task<IGraphQLHttpBatchExecutorResponse> ExecuteHttpBatch(
            IGraphQLBatchRequest request,
            IEnumerable<IHttpBatchRequest> batchRequests,
            CancellationToken cancellationToken);

        Task<IGraphQLSubscriptionResponseOperation<TInput, TOutput>> ExecuteSubscription<TInput, TOutput>(
            IGraphQLSubscriptionRequest<TInput> request,
            Func<GraphQLField, bool> queryFilter,
            Expression<Func<TInput, TOutput>> outputSelector,
            CancellationToken cancellationToken)
            where TInput : class
            where TOutput : class;
    }
}
