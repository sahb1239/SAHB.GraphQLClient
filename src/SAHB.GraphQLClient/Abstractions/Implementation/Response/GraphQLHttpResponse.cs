using System.Collections.Generic;
using System.Net;
using SAHB.GraphQLClient.Execution;

namespace SAHB.GraphQLClient
{
    public class GraphQLHttpResponse<TInput, TOutput> :
        GraphQLResponse<TInput, TOutput, IGraphQLHttpRequest<TInput>>,
        IGraphQLHttpResponse<TInput, TOutput, IGraphQLHttpRequest<TInput>>
        where TInput : class
        where TOutput : class
    {
        public GraphQLHttpResponse(IGraphQLHttpRequest<TInput> request, IGraphQLHttpExecutorResponse<TInput, TOutput> response)
            : base(request, response)
        {
            Headers = response.Headers;
            StatusCode = response.StatusCode;
        }

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

        public HttpStatusCode StatusCode { get; }
    }
}
