using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient
{
    public class GraphQLBatchHttpRequest<TInput, TOutput> : GraphQLRequest<TInput>, IGraphQLBatchHttpRequest<TInput, TOutput>
        where TInput : class
        where TOutput : class
    {
        private Expression<Func<TInput, TOutput>> filter;

        public GraphQLBatchHttpRequest(IGraphQLClient graphQLClient, IEnumerable<GraphQLField> selectionSet, GraphQLOperationType operationType, Expression<Func<TInput, TOutput>> filter, Task<IGraphQLHttpResponse<TInput, TOutput, IGraphQLBatchHttpRequest<TInput, TOutput>>> executionOutput) : base(graphQLClient, selectionSet, operationType)
        {
            Filter = filter;
            ExecutionOutput = executionOutput;
        }

        public Expression<Func<TInput, TOutput>> Filter
        {
            get => filter;
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                filter = value;
            }
        }

        private Task<IGraphQLHttpResponse<TInput, TOutput, IGraphQLBatchHttpRequest<TInput, TOutput>>> ExecutionOutput { get; }

        public Task<IGraphQLHttpResponse<TInput, TOutput, IGraphQLBatchHttpRequest<TInput, TOutput>>> Execute(CancellationToken cancellationToken = default)
        {
            return ExecutionOutput;
        }
    }
}
