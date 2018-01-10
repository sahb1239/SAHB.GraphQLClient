using SAHB.GraphQLClient.QueryGenerator;
namespace SAHB.GraphQLClient.Batching
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL batch which supports querying multiple queries at one time
    /// </summary>
    public interface IGraphQLBatch
    {
        /// <summary>
        /// Generates a query to a GraphQL server using a specified type and the GraphQL argument variables
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns></returns>
        IGraphQLQuery<T> Query<T>(params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Returns true if the batch has been executed, when executed its not possible to add more queries to it
        /// </summary>
        /// <returns>Returns if the batch has been executed</returns>
        bool IsExecuted();
    }
}