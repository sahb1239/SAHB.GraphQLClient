using System.Collections.Generic;
using System.Net;

namespace SAHB.GraphQLClient
{
    public interface IGraphQLHttpResponse<in TInput, out TOutput, out TInputRequest>
        : IGraphQLResponse<TInput, TOutput, TInputRequest>
        where TInput : class
        where TOutput : class
        where TInputRequest : IGraphQLRequest<object>
    {
        IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }
        HttpStatusCode StatusCode { get; }
    }
}
