using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient
{
    public abstract class GraphQLResponse<TInput, TOutput, TInputRequest> : IGraphQLResponse<TInput, TOutput, TInputRequest>
        where TInput : class
        where TOutput : class
        where TInputRequest : IGraphQLRequest<TInput>
    {
        public GraphQLResponse(IGraphQLClient client, TInputRequest request, string query, string response, Expression<Func<TInput, TOutput>> filter)
        {
            this.Response = response;
            this.Filter = filter ?? throw new ArgumentNullException(nameof(filter));
            Client = client;
            this.Request = request;
            this.Query = query;

            DeserilizeResponse();
        }

        private void DeserilizeResponse()
        {
            // Deserilize
            var deserilizationResult = Client.Deserialization.DeserializeResult<TInput>(GetRawOutput(), Request.SelectionSet);

            if (deserilizationResult?.Errors?.Any() ?? false)
                throw new GraphQLErrorException(query: Query, errors: deserilizationResult.Errors);

            // Set data
            Data = GetOutputData(deserilizationResult.Data);
            Errors = new ReadOnlyCollection<GraphQLDataError>(deserilizationResult.Errors.ToList());
            AdditionalData = new ReadOnlyDictionary<string, object>(deserilizationResult.AdditionalData.ToDictionary(e => e.Key, e => (object)e.Value));
        }

        private TOutput GetOutputData(TInput input)
        {
            var compiledFilter = Filter.Compile();
            return compiledFilter(input);
        }

        public IGraphQLClient Client { get; }

        public TInputRequest Request { get; }
        public string Query { get; }

        protected string Response { get; }

        public Expression<Func<TInput, TOutput>> Filter { get; }

        public TOutput Data { get; private set; }

        public IReadOnlyCollection<GraphQLDataError> Errors { get; private set; }

        public IReadOnlyDictionary<string, object> AdditionalData { get; private set; }

        public string GetRawOutput()
        {
            return Response;
        }
    }
}
