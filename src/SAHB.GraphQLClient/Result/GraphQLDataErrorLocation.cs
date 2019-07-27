using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SAHB.GraphQLClient.Result
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// The location which a GraphQL error occured
    /// </summary>
    public class GraphQLDataErrorLocation
    {
        /// <summary>
        /// The line which the error occured
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// The column which the error occured
        /// </summary>
        public int Column { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }
    }
}