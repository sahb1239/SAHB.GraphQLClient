using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient
{
    public interface IGraphQLSubscriptionRequest<TInput>
        : IGraphQLRequest<TInput>
        where TInput : class
    {
        Task<IGraphQLSubscriptionResponseOperation<TInput, TInput>> Execute(CancellationToken cancellationToken = default);
        Task<IGraphQLSubscriptionResponseOperation<TInput, TOutput>> Execute<TOutput>(
            Expression<Func<TInput, TOutput>> filter, CancellationToken cancellationToken = default)
            where TOutput : class;
    }
}
