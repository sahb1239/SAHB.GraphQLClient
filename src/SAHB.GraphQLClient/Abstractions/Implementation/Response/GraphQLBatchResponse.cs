using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;

namespace SAHB.GraphQLClient
{
    public partial class GraphQLBatchRequest
    {
        public class GraphQLBatchResponse<TInput, TOutput> : GraphQLResponse<TInput, TOutput, IGraphQLBatchHttpRequest<TInput, TOutput>>, IGraphQLHttpResponse<TInput, TOutput, IGraphQLBatchHttpRequest<TInput, TOutput>>
            where TInput : class
            where TOutput : class
        {
            public GraphQLBatchResponse(IGraphQLClient client, IGraphQLBatchHttpRequest<TInput, TOutput> request, string query, string response, Expression<Func<TInput, TOutput>> filter, HttpResponseHeaders headers, HttpStatusCode statusCode)
                : base(client, request, query, response, filter)
            {
                Headers = new ReadOnlyDictionary<string, IEnumerable<string>>(
                    headers?.ToDictionary(e => e.Key, e => e.Value) ?? new Dictionary<string, IEnumerable<string>>());
                StatusCode = statusCode;
            }

            public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

            public HttpStatusCode StatusCode { get; }
        }
    }
}
