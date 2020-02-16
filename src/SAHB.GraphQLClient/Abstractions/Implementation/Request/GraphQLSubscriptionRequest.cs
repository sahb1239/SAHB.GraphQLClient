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
            var query = GetQuery();
            return Execute<T>(query, cancellationToken);
        }

        public Task<IGraphQLSubscriptionResponseOperation<T, TOutput>> Execute<TOutput>(Expression<Func<T, TOutput>> filter, CancellationToken cancellationToken = default) where TOutput : class
        {
            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var query = GetQuery(filter);
            return Execute<TOutput>(query, cancellationToken);
        }

        private Task<IGraphQLSubscriptionResponseOperation<T, TOutput>> Execute<TOutput>(string query, CancellationToken cancellationToken)
            where TOutput : class
        {
            throw new NotImplementedException();
        }
    }
}
