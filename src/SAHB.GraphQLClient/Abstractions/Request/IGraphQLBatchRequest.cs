using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Builder;

namespace SAHB.GraphQLClient
{
    public interface IGraphQLBatchRequest : IGraphQLRequestInformation
    {
        IGraphQLBatchHttpRequest<T, T> CreateHttpRequest<T>()
            where T : class;
        IGraphQLBatchHttpRequest<TInput, TOutput> CreateHttpRequest<TInput, TOutput>(Expression<Func<TInput, TOutput>> filter)
            where TInput : class
            where TOutput : class;
        IGraphQLBatchHttpRequest<dynamic, dynamic> CreateHttpRequest(Action<IGraphQLBuilder> queryBuilder);
        Task Execute(CancellationToken cancellationToken = default);
    }
}
