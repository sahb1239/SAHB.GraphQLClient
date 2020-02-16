using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient
{
    public static class GraphQLHttpRequestExtensions
    {
        #region Execute

        public static async Task<TInput> GetData<TInput>(this IGraphQLHttpRequest<TInput> request, CancellationToken cancellationToken = default)
            where TInput : class
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await request.Execute(cancellationToken).ConfigureAwait(false);

            return response.Data;
        }

        public static async Task<TOutput> GetData<TInput, TOutput>(this IGraphQLBatchHttpRequest<TInput, TOutput> request, CancellationToken cancellationToken = default)
            where TInput : class
            where TOutput : class
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var response = await request.Execute(cancellationToken).ConfigureAwait(false);

            return response.Data;
        }

        public static async Task<TOutput> GetData<TInput, TOutput>(this IGraphQLHttpRequest<TInput> request, Expression<Func<TInput, TOutput>> filter, CancellationToken cancellationToken = default)
            where TInput : class
            where TOutput : class
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var response = await request.Execute(filter, cancellationToken).ConfigureAwait(false);

            return response.Data;
        }

        #endregion
    }
}
