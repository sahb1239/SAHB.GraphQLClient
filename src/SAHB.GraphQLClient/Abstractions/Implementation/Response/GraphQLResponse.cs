using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Execution;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient
{
    public abstract class GraphQLResponse<TInput, TOutput, TInputRequest> : IGraphQLResponse<TInput, TOutput, TInputRequest>
        where TInput : class
        where TOutput : class
        where TInputRequest : IGraphQLRequest<TInput>
    {
        public GraphQLResponse(TInputRequest request, IGraphQLExecutorResponse<TInput, TOutput> response)
        {
            this.Response = response;
            this.Request = request;
            this.Query = response.Query;

            CheckDeserilizationResult();
            SetData();
        }

        private void CheckDeserilizationResult()
        {
            var deserilizationResult = Response.OutputDeserilizedResponse;
            if (deserilizationResult.Errors?.Any() ?? false)
                throw new GraphQLErrorException(query: Query, errors: deserilizationResult.Errors);
        }

        private void SetData()
        {
            // Set data
            Data = Response.OutputDeserilizedResponse.Data;
            Errors = new ReadOnlyCollection<GraphQLDataError>(Response.OutputDeserilizedResponse.Errors.ToList());
            AdditionalData = new ReadOnlyDictionary<string, object>(Response.OutputDeserilizedResponse.AdditionalData.ToDictionary(e => e.Key, e => (object)e.Value));
        }

        public TInputRequest Request { get; }
        public string Query { get; }

        protected IGraphQLExecutorResponse<TInput, TOutput> Response { get; }

        public TOutput Data { get; private set; }

        public IReadOnlyCollection<GraphQLDataError> Errors { get; private set; }

        public IReadOnlyDictionary<string, object> AdditionalData { get; private set; }

        public string GetRawOutput()
        {
            return Response.Response;
        }
    }
}
