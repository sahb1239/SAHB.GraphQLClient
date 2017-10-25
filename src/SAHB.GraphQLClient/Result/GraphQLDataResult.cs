using System.Collections.Generic;

namespace SAHB.GraphQLClient.Result
{
    // ReSharper disable once InconsistentNaming
    public class GraphQLDataResult<T> where T : class
    {
        public T Data { get; set; }
        public IEnumerable<GraphQLDataError> Errors { get; set; }
    }
}