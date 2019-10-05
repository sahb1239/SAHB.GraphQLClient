using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace SAHB.GraphQLClient.Result
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Contains the GraphQL data result
    /// </summary>
    /// <typeparam name="T">The data type returned</typeparam>
    public class GraphQLDataResult<T> where T : class
    {
        /// <summary>
        /// Contains the output from the GraphQL server. This is null, when errors has occured
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Contains the response headers
        /// </summary>
        public HttpResponseHeaders Headers { get; set; }

        /// <summary>
        /// The errors which occured on execution of the query
        /// </summary>
        public IEnumerable<GraphQLDataError> Errors { get; set; }

        /// <summary>
        /// Returns true if the result contains errors
        /// </summary>
        public bool ContainsErrors => Errors?.Any() ?? false;

        /// <summary>
        /// Return true if the result contains data
        /// </summary>
        public bool ContainsData => Data != null;

        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }
    }
}
