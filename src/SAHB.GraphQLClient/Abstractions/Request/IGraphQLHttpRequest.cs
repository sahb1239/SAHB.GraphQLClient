using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient
{
    public interface IGraphQLHttpRequest<TInput>
        : IGraphQLRequest<TInput>
        where TInput : class
    {
        HttpMethod Method { get; set; }

        Task<IGraphQLHttpResponse<TInput, TInput, IGraphQLHttpRequest<TInput>>> Execute(CancellationToken cancellationToken = default);
        Task<IGraphQLHttpResponse<TInput, TOutput, IGraphQLHttpRequest<TInput>>> Execute<TOutput>(
            Expression<Func<TInput, TOutput>> filter, CancellationToken cancellationToken = default)
            where TOutput : class;
    }
}
