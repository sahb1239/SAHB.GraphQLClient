namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    public class GraphQLDataResult<T> where T : class
    {
        public T Data { get; set; }
    }
}