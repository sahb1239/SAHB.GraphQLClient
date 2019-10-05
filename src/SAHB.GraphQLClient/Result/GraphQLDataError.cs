using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace SAHB.GraphQLClient.Result
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL error
    /// </summary>
    public class GraphQLDataError
    {
        /// <summary>
        /// Description of the error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The locations the errors occured
        /// </summary>
        public IEnumerable<GraphQLDataErrorLocation> Locations { get; set; }

        /// <summary>
        /// Returns true if the GraphQL error contains locations
        /// </summary>
        public bool ContainLocations => Locations?.Any() ?? false;

        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }
    }
}
