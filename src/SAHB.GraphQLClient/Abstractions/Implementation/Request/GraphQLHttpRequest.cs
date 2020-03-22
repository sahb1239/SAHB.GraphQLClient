using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient
{
    public class GraphQLHttpRequest<TInput> : GraphQLRequest<TInput>, IGraphQLHttpRequest<TInput>
        where TInput : class
    {
        public GraphQLHttpRequest(GraphQLClient graphQLClient, IEnumerable<GraphQLField> selectionSet, GraphQLOperationType operationType)
            : base(graphQLClient, selectionSet, operationType)
        {
        }

        public HttpMethod Method { get; set; }

        public Task<IGraphQLHttpResponse<TInput, TInput, IGraphQLHttpRequest<TInput>>> Execute(CancellationToken cancellationToken = default)
        {
            return Execute(e => e, cancellationToken);
        }

        public async Task<IGraphQLHttpResponse<TInput, TOutput, IGraphQLHttpRequest<TInput>>> Execute<TOutput>(
            Expression<Func<TInput, TOutput>> filter,
            CancellationToken cancellationToken = default) where TOutput : class
        {
            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            // Get Response
            var response = await this.Client.Executor.ExecuteHttp(this,
                    GetQueryFilter(filter),
                    filter,
                    cancellationToken)
                .ConfigureAwait(false);

            return response;
        }
    }
}
