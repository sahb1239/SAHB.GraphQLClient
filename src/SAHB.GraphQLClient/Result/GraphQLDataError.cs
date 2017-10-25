using System.Collections.Generic;

namespace SAHB.GraphQLClient.Result
{
    // ReSharper disable once InconsistentNaming
    public class GraphQLDataError
    {
        public string Message { get; set; }
        public IEnumerable<GraphQLDataErrorLocation> Locations { get; set; }
    }
}