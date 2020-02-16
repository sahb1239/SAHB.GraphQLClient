using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient
{
    public partial class GraphQLBatchRequest : GraphQLRequestInformation, IGraphQLBatchRequest
    {
        public GraphQLBatchRequest(IGraphQLClient graphQLClient, GraphQLOperationType operationType)
            : base(graphQLClient, operationType)
        {
        }

        public IGraphQLBatchHttpRequest<T, T> CreateHttpRequest<T>() where T : class
        {
            return CreateRequest<T, T>(Client.GetSelectionSet<T>(), e => e);
        }

        public IGraphQLBatchHttpRequest<TInput, TOutput> CreateHttpRequest<TInput, TOutput>(Expression<Func<TInput, TOutput>> filter)
            where TInput : class
            where TOutput : class
        {
            return CreateRequest(Client.GetSelectionSet<TInput>(), filter);
        }

        public IGraphQLBatchHttpRequest<dynamic, dynamic> CreateHttpRequest(Action<IGraphQLBuilder> queryBuilder)
        {
            return CreateRequest<dynamic, dynamic>(queryBuilder.GetSelectionSet(), e => e);
        }

        public Task Execute(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private IGraphQLBatchHttpRequest<TInput, TOutput> CreateRequest<TInput, TOutput>(IEnumerable<GraphQLField> selectionSet, Expression<Func<TInput, TOutput>> filter)
            where TInput : class
            where TOutput : class
        {
            var request = new BatchRequest<TInput, TOutput>(Client, Operation, selectionSet, filter);
            BatchRequests.Add(request);
            return request.Request;
        }

        private List<BatchRequest> BatchRequests { get; } = new List<BatchRequest>();


        private abstract class BatchRequest
        {
            public abstract IEnumerable<GraphQLField> GetSelectionSet();
            public abstract IEnumerable<GraphQLQueryArgument> GetArguments();
            public abstract IEnumerable<GraphQLQueryArgument> GetDirectiveArguments();

            public abstract void SetExecutionResult(string query, string response, HttpResponseHeaders headers, HttpStatusCode statusCode);

            public abstract void SetException(IEnumerable<Exception> exceptions);

            public abstract void SetCancelled();
        }

        private class BatchRequest<TInput, TOutput> : BatchRequest
            where TInput : class
            where TOutput : class
        {
            public BatchRequest(IGraphQLClient client, GraphQLOperationType operation, IEnumerable<GraphQLField> selectionSet, Expression<Func<TInput, TOutput>> filter)
            {
                Request =
                    new GraphQLBatchHttpRequest<TInput, TOutput>(
                        client,
                        selectionSet,
                        operation,
                        filter,
                        this.ExecutionResult.Task);
            }

            public GraphQLBatchHttpRequest<TInput, TOutput> Request { get; set; }

            public TaskCompletionSource<IGraphQLHttpResponse<TInput, TOutput, IGraphQLBatchHttpRequest<TInput, TOutput>>> ExecutionResult { get; }
                = new TaskCompletionSource<IGraphQLHttpResponse<TInput, TOutput, IGraphQLBatchHttpRequest<TInput, TOutput>>>();

            public override IEnumerable<GraphQLQueryArgument> GetArguments() => Request.Arguments;

            public override IEnumerable<GraphQLQueryArgument> GetDirectiveArguments() => Request.DirectiveArguments;

            public override IEnumerable<GraphQLField> GetSelectionSet() => Request.SelectionSet;

            public override void SetCancelled()
            {
                ExecutionResult.SetCanceled();
            }

            public override void SetException(IEnumerable<Exception> exceptions)
            {
                ExecutionResult.SetException(exceptions);
            }

            public override void SetExecutionResult(string query, string response, HttpResponseHeaders headers, HttpStatusCode statusCode)
            {
                ExecutionResult.SetResult(new GraphQLBatchResponse<TInput, TOutput>(Request.Client, Request, query, response, Request.Filter, headers, statusCode));
            }
        }
    }
}
