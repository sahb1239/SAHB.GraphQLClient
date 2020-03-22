using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Execution
{
    public class GraphQLExecutor : IGraphQLExecutor
    {
        /// <summary>
        /// The <see cref="IGraphQLQueryGeneratorFromFields"/> used
        /// </summary>
        public IGraphQLQueryGeneratorFromFields QueryGenerator { get; set; }

        /// <summary>
        /// The <see cref="IGraphQLDeserialization"/> used
        /// </summary>
        public IGraphQLDeserialization Deserialization { get; set; }

        /// <summary>
        /// The <see cref="IGraphQLHttpExecutor"/> used
        /// </summary>
        public IGraphQLHttpExecutor HttpExecutor { get; set; }

        /// <summary>
        /// The <see cref="IGraphQLSubscriptionExecutor"/> used
        /// </summary>
        public IGraphQLSubscriptionExecutor SubscriptionExecutor { get; set; }

        public GraphQLExecutor(IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLDeserialization deserialization, IGraphQLHttpExecutor httpExecutor, IGraphQLSubscriptionExecutor subscriptionExecutor)
        {
            QueryGenerator = queryGenerator ?? throw new ArgumentNullException(nameof(queryGenerator));
            Deserialization = deserialization ?? throw new ArgumentNullException(nameof(deserialization));
            HttpExecutor = httpExecutor ?? throw new ArgumentNullException(nameof(httpExecutor));
            SubscriptionExecutor = subscriptionExecutor ?? throw new ArgumentNullException(nameof(subscriptionExecutor));
        }

        #region ExecuteHttp

        public async Task<IGraphQLHttpResponse<TInput, TOutput, IGraphQLHttpRequest<TInput>>> ExecuteHttp<TInput, TOutput>(IGraphQLHttpRequest<TInput> request, Func<GraphQLField, bool> queryFilter, Expression<Func<TInput, TOutput>> outputSelector, CancellationToken cancellationToken)
            where TInput : class
            where TOutput : class
        {
            await EnsureValidated(request, cancellationToken).ConfigureAwait(false);

            return await ExecuteAndProcessHttpResponse(request, queryFilter, outputSelector, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IGraphQLHttpResponse<TInput, TOutput, IGraphQLHttpRequest<TInput>>> ExecuteAndProcessHttpResponse<TInput, TOutput>(IGraphQLHttpRequest<TInput> request, Func<GraphQLField, bool> queryFilter, Expression<Func<TInput, TOutput>> outputSelector, CancellationToken cancellationToken)
            where TInput : class
            where TOutput : class
        {
            var query = GetQuery(request, queryFilter);
            var response = await ExecuteHttpQuery(request, query, cancellationToken).ConfigureAwait(false);
            return ProcessHttpResponse(request, response, outputSelector);
        }

        protected virtual Task<GraphQLExecutorResponse> ExecuteHttpQuery<TInput>(IGraphQLHttpRequest<TInput> request, string query, CancellationToken cancellationToken)
            where TInput : class
        {
            return HttpExecutor.ExecuteQuery(query, request.Url, request.Method, request.AuthorizationToken, request.AuthorizationMethod, request.Headers, cancellationToken);
        }

        protected virtual IGraphQLHttpResponse<TInput, TOutput, IGraphQLHttpRequest<TInput>> ProcessHttpResponse<TInput, TOutput>(IGraphQLHttpRequest<TInput> request, GraphQLExecutorResponse response, Expression<Func<TInput, TOutput>> outputSelector)
            where TInput : class
            where TOutput : class
        {
            throw new NotImplementedException();
            //return new GraphQLHttpResponse<TInput, TOutput>(request, )
        }

        #endregion

        #region ExecuteSubscription

        public async Task<IGraphQLSubscriptionResponseOperation<TInput, TOutput>> ExecuteSubscription<TInput, TOutput>(IGraphQLSubscriptionRequest<TInput> request, Func<GraphQLField, bool> queryFilter, Expression<Func<TInput, TOutput>> outputSelector, CancellationToken cancellationToken)
            where TInput : class
            where TOutput : class
        {
            await EnsureValidated(request, cancellationToken).ConfigureAwait(false);

            return await ExecuteAndProcessSubscriptionResponse(request, queryFilter, outputSelector, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IGraphQLSubscriptionResponseOperation<TInput, TOutput>> ExecuteAndProcessSubscriptionResponse<TInput, TOutput>(IGraphQLSubscriptionRequest<TInput> request, Func<GraphQLField, bool> queryFilter, Expression<Func<TInput, TOutput>> outputSelector, CancellationToken cancellationToken)
            where TInput : class
            where TOutput : class
        {
            var query = GetQuery(request, queryFilter);
            var response = await ExecuteSubscriptionQuery(request, query, cancellationToken).ConfigureAwait(false);
            return ProcessSubscriptionResponse(request, response, outputSelector);
        }

        protected virtual Task<GraphQLSubscriptionExecutorResponse> ExecuteSubscriptionQuery<TInput>(IGraphQLSubscriptionRequest<TInput> request, string query, CancellationToken cancellationToken)
            where TInput : class
        {
            return SubscriptionExecutor.ExecuteQuery(query, request.Url, request.AuthorizationToken, request.AuthorizationMethod, request.Headers, cancellationToken);
        }

        protected virtual IGraphQLSubscriptionResponseOperation<TInput, TOutput> ProcessSubscriptionResponse<TInput, TOutput>(IGraphQLSubscriptionRequest<TInput> request, GraphQLSubscriptionExecutorResponse response, Expression<Func<TInput, TOutput>> outputSelector)
            where TInput : class
            where TOutput : class
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ExecuteHttpBatch

        public async Task<IGraphQLHttpBatchExecutorResponse> ExecuteHttpBatch(IGraphQLBatchRequest request, IEnumerable<IHttpBatchRequest> batchRequests, CancellationToken cancellationToken)
        {
            var requests = CreateBatchRequests(batchRequests);
            await AddValidationInfoToBatchRequests(request, requests, cancellationToken).ConfigureAwait(false);
            return await ExecuteAndProcessBatchResponse(request, requests, cancellationToken).ConfigureAwait(false);
        }

        protected virtual List<BatchRequest> CreateBatchRequests(IEnumerable<IHttpBatchRequest> batchRequests)
        {
            return batchRequests.Select((request, index) => new BatchRequest($"batch_{index}", request)).ToList();
        }

        protected virtual async ValueTask AddValidationInfoToBatchRequests(IGraphQLBatchRequest request, ICollection<BatchRequest> batchRequests, CancellationToken cancellationToken)
        {
            if (request.ShouldThrowIfQueryIsInvalid == true)
            {
                var introspectionSchema = await request.GetIntrospectionSchema(cancellationToken).ConfigureAwait(false);
                foreach (var batch in batchRequests)
                {
                    batch.ValidationErrors = batch.Batch.Validate(introspectionSchema);
                }
            }
        }

        private async Task<IGraphQLHttpBatchExecutorResponse> ExecuteAndProcessBatchResponse(IGraphQLBatchRequest request, ICollection<BatchRequest> batchRequests, CancellationToken cancellationToken)
        {
            var query = GetQuery(request, batchRequests);
            var response = await ExecuteBatchQuery(request, query, cancellationToken).ConfigureAwait(false);
            return ProcessBatchResponse(request, batchRequests, response);
        }

        protected virtual Task<GraphQLExecutorResponse> ExecuteBatchQuery(IGraphQLBatchRequest request, string query, CancellationToken cancellationToken)
        {
            return HttpExecutor.ExecuteQuery(query, request.Url, request.Method, request.AuthorizationToken, request.AuthorizationMethod, request.Headers, cancellationToken);
        }

        protected virtual IGraphQLHttpBatchExecutorResponse ProcessBatchResponse(IGraphQLBatchRequest request, ICollection<BatchRequest> batchRequests, GraphQLExecutorResponse response)
        {
            throw new NotImplementedException();
        }

        protected class BatchRequest
        {
            public BatchRequest(string prefix, IHttpBatchRequest batch)
            {
                Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
                Batch = batch ?? throw new ArgumentNullException(nameof(batch));
            }

            public string Prefix { get; }
            public IHttpBatchRequest Batch { get; }
            
            public bool IsValid
            {
                get
                {
                    if (ValidationErrors == null)
                        return true;

                    return (!ValidationErrors.Any());
                }
            }
            
            public IEnumerable<ValidationError> ValidationErrors { get; set; }
        }

        #endregion

        #region GetQuery

        /// <summary>
        /// Returns the query for the input request
        /// </summary>
        /// <typeparam name="TInput">The request type</typeparam>
        /// <param name="request">The request which the query should be generated for</param>
        /// <param name="queryFilter">The query filter</param>
        /// <returns></returns>
        protected virtual string GetQuery<TInput>(IGraphQLRequest<TInput> request, Func<GraphQLField, bool> queryFilter)
            where TInput : class
        {
            return GetQuery(request.Operation, request.SelectionSet, queryFilter, request.Arguments, request.DirectiveArguments);
        }

        protected virtual string GetQuery(IGraphQLBatchRequest request, ICollection<BatchRequest> batchRequests)
        {
            throw new NotImplementedException();
        }

        private string GetQuery(GraphQLOperationType operation, IReadOnlyCollection<GraphQLField> selectionSet, Func<GraphQLField, bool> queryFilter, ICollection<GraphQLQueryArgument> arguments, ICollection<GraphQLQueryArgument> directiveArguments)
        {
            return QueryGenerator.GenerateQuery(operation, selectionSet, queryFilter, arguments.Concat(directiveArguments).ToArray());
        }

        #endregion

        #region Validation

        /// <summary>
        /// Ensures that the request is validated if <see cref="IGraphQLRequestInformation.ShouldThrowIfQueryIsInvalid"/> is set to true
        /// </summary>
        /// <typeparam name="TInput">The request type</typeparam>
        /// <param name="request">The request which should be validated</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual async ValueTask EnsureValidated<TInput>(IGraphQLRequest<TInput> request, CancellationToken cancellationToken)
            where TInput : class
        {
            if (request.ShouldThrowIfQueryIsInvalid == true)
            {
                await Validate(request, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task Validate<TInput>(IGraphQLRequest<TInput> request, CancellationToken cancellationToken) where TInput : class
        {
            await request.ThrowIfNotValid(cancellationToken).ConfigureAwait(false);
        }

        #endregion
    }
}
