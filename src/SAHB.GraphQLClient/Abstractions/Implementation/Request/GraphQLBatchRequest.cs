using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.Execution;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient
{
    public partial class GraphQLBatchRequest : GraphQLRequestInformation, IGraphQLBatchRequest
    {
        public GraphQLBatchRequest(IGraphQLClient graphQLClient, GraphQLOperationType operationType)
            : base(graphQLClient, operationType)
        {
        }

        public HttpMethod Method { get; set; }

        #region CreateHttpRequest

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

        private IGraphQLBatchHttpRequest<TInput, TOutput> CreateRequest<TInput, TOutput>(IEnumerable<GraphQLField> selectionSet, Expression<Func<TInput, TOutput>> filter)
            where TInput : class
            where TOutput : class
        {
            var request = new BatchRequest<TInput, TOutput>(Client, Operation, selectionSet, filter);
            BatchRequests.Add(request);
            return request.Request;
        }

        #endregion

        #region Execute

        public async Task Execute(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await ExecuteBatch(cancellationToken);
                SetExecutionResult(response);
            }
            catch(TaskCanceledException)
            {
                SetCancelled();
            }
            catch(Exception ex)
            {
                SetException(new[] { ex });
            }
        }

        private Task<IGraphQLHttpBatchExecutorResponse> ExecuteBatch(CancellationToken cancellationToken)
        {
            return this.Client.Executor.ExecuteHttpBatch(this, BatchRequests, cancellationToken);
        }

        private void SetExecutionResult(IGraphQLHttpBatchExecutorResponse executionResult)
        {
            DoOnAllRequests(request => request.SetExecutionResult(executionResult));
        }

        private void SetCancelled()
        {
            DoOnAllRequests(request => request.SetCancelled());
        }

        private void SetException(IEnumerable<Exception> exceptions)
        {
            DoOnAllRequests(request => request.SetException(exceptions));
        }

        private void DoOnAllRequests(Action<BatchRequest> action)
        {
            List<Exception> exceptions = new List<Exception>();
            foreach (var request in BatchRequests)
            {
                try
                {
                    action(request);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }

        #endregion

        private List<BatchRequest> BatchRequests { get; } = new List<BatchRequest>();

        private abstract class BatchRequest : IHttpBatchRequest
        {
            public abstract IReadOnlyCollection<GraphQLField> GetSelectionSet();
            public abstract ICollection<GraphQLQueryArgument> GetArguments();
            public abstract ICollection<GraphQLQueryArgument> GetDirectiveArguments();

            public abstract Func<GraphQLField, bool> GetQueryFilter();
            public abstract IEnumerable<ValidationError> Validate(GraphQLIntrospectionSchema graphQLIntrospectionSchema);

            public abstract void SetExecutionResult(IGraphQLHttpBatchExecutorResponse response);

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

            public override ICollection<GraphQLQueryArgument> GetArguments() => Request.Arguments;

            public override ICollection<GraphQLQueryArgument> GetDirectiveArguments() => Request.DirectiveArguments;

            public override Func<GraphQLField, bool> GetQueryFilter()
            {
                return Request.GetQueryFilter(Request.Filter);
            }

            public override IReadOnlyCollection<GraphQLField> GetSelectionSet() => Request.SelectionSet;

            public override IEnumerable<ValidationError> Validate(GraphQLIntrospectionSchema graphQLIntrospectionSchema) => Request.Validate(graphQLIntrospectionSchema);

            public override void SetCancelled()
            {
                ExecutionResult.SetCanceled();
            }

            public override void SetException(IEnumerable<Exception> exceptions)
            {
                ExecutionResult.SetException(exceptions);
            }

            public override void SetExecutionResult(IGraphQLHttpBatchExecutorResponse response)
            {
                ExecutionResult.SetResult(response.GetResponse<TInput, TOutput>(Request));
            }
        }
    }
}
