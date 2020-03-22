using System.Collections.Generic;
using System.Net;
using SAHB.GraphQLClient.Execution;

namespace SAHB.GraphQLClient
{
    public partial class GraphQLBatchRequest
    {
        public class GraphQLBatchResponse<TInput, TOutput> : GraphQLResponse<TInput, TOutput, IGraphQLBatchHttpRequest<TInput, TOutput>>, IGraphQLHttpResponse<TInput, TOutput, IGraphQLBatchHttpRequest<TInput, TOutput>>
            where TInput : class
            where TOutput : class
        {
            public GraphQLBatchResponse(IGraphQLBatchHttpRequest<TInput, TOutput> request, IGraphQLHttpExecutorResponse<TInput, TOutput> response)
                : base(request, response)
            {
                Headers = response.Headers;
                StatusCode = response.StatusCode;
            }

            public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

            public HttpStatusCode StatusCode { get; }
        }
    }
}
