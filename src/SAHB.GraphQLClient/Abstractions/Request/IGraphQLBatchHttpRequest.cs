using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient
{
    public interface IGraphQLBatchHttpRequest<TInput, TOutput>
        : IGraphQLRequest<TInput>
        where TInput : class
        where TOutput : class
    {
        Expression<Func<TInput, TOutput>> Filter { get; set; }

        Task<IGraphQLHttpResponse<TInput, TOutput, IGraphQLBatchHttpRequest<TInput, TOutput>>> Execute(CancellationToken cancellationToken = default);
    }
}
