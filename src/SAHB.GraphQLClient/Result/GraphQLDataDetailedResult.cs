using System.Net.Http.Headers;

namespace SAHB.GraphQLClient.Result
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Contains the GraphQL data result with response headers
    /// </summary>
    /// <typeparam name="T">The data type returned</typeparam>
    public class GraphQLDataDetailedResult<T>
    {
        /// <summary>
        /// Contains the output from the GraphQL server. This is null, when errors has occured
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Contains the response headers
        /// </summary>
        public HttpResponseHeaders Headers { get; set; }
    }
}