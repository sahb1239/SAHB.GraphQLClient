using System;
using System.Collections.Generic;
using System.Net;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Execution
{
    public class GraphQLExecutorResponse<TInput, TOutput> : IGraphQLExecutorResponse<TInput, TOutput>
        where TInput : class
        where TOutput : class
    {
        public string Query { get; set; }
        public string Response { get; private set; }
        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }

        public Exception DeserilizationException { get; private set; }

        public GraphQLDataResult<TInput> InputDeserilizedResponse { get; private set; }

        public GraphQLDataResult<TOutput> OutputDeserilizedResponse { get; private set; }
    }
}
