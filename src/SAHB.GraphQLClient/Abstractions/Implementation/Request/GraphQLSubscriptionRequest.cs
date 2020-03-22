using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient
{
    public class GraphQLSubscriptionRequest<T> : GraphQLRequest<T>, IGraphQLSubscriptionRequest<T>
        where T : class
    {
        public GraphQLSubscriptionRequest(GraphQLClient graphQLClient, IEnumerable<GraphQLField> selectionSet, GraphQLOperationType operationType)
            : base(graphQLClient, selectionSet, operationType)
        {
        }

        public Task<IGraphQLSubscriptionResponseOperation<T, T>> Execute(CancellationToken cancellationToken = default)
        {
            return Execute(e => e, cancellationToken);
        }

        public async Task<IGraphQLSubscriptionResponseOperation<T, TOutput>> Execute<TOutput>(Expression<Func<T, TOutput>> filter, CancellationToken cancellationToken = default) where TOutput : class
        {
            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            // Get Response
            var response = await this.Client.Executor.ExecuteSubscription(this,
                    GetQueryFilter(filter),
                    filter,
                    cancellationToken)
                .ConfigureAwait(false);

            return response;
        }
    }
}
