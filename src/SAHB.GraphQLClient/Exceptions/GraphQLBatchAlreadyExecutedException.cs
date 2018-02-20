namespace SAHB.GraphQLClient.Exceptions
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Throws a new <see cref="GraphQLBatchAlreadyExecutedException"/> which indicates the batch has already been executed
    /// </summary>
    public class GraphQLBatchAlreadyExecutedException : GraphQLException
    {
        public GraphQLBatchAlreadyExecutedException() : base("The GraphQL batch has already been executed")
        {
        }
    }
}
