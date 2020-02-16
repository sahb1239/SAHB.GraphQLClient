using System.Net;
using System.Net.Http.Headers;

namespace SAHB.GraphQLClient.Executor
{
    public class GraphQLExecutorResponse
    {
        public string Response { get; set; }
        public HttpResponseHeaders Headers { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
