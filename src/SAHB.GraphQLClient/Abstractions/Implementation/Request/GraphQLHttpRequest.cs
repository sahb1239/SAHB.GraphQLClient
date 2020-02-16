using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Builder;
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
            var query = GetQuery();
            return Execute(query, e => e, cancellationToken);
        }

        public Task<IGraphQLHttpResponse<TInput, TOutput, IGraphQLHttpRequest<TInput>>> Execute<TOutput>(
            Expression<Func<TInput, TOutput>> filter,
            CancellationToken cancellationToken = default) where TOutput : class
        {
            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var query = GetQuery(filter);
            return Execute(query, filter, cancellationToken);
        }

        private async Task<IGraphQLHttpResponse<TInput, TOutput, IGraphQLHttpRequest<TInput>>> Execute<TOutput>(string query, Expression<Func<TInput, TOutput>> filter, CancellationToken cancellationToken)
            where TOutput : class
        {
            // Get Response
            var response = await this.Client.HttpExecutor.ExecuteQuery(query, Url, Method, AuthorizationToken, AuthorizationMethod, Headers, cancellationToken)
                .ConfigureAwait(false);

            return new GraphQLHttpResponse<TInput, TOutput>(Client, this, query, response.Response, filter, response.Headers, response.StatusCode);
        }
    }
}
