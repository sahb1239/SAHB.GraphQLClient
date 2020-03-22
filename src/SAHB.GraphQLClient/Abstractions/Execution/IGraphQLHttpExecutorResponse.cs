using System.Collections.Generic;
using System.Net;

namespace SAHB.GraphQLClient.Execution
{
    public interface IGraphQLHttpExecutorResponse<TInput, TOutput>
        : IGraphQLExecutorResponse<TInput, TOutput>
        where TInput : class
        where TOutput : class
    {
        IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }
        HttpStatusCode StatusCode { get; }
    }
}
