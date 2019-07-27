using System.Threading.Tasks;

namespace SAHB.GraphQLClient
{
    using Result;

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Generated Query which supports executing the query
    /// </summary>
    public interface IGraphQLQuery
    {
        /// <summary>
        /// Execute the query
        /// </summary>
        Task<dynamic> Execute();

        /// <summary>
        /// Execute query and return the result with response headers
        /// </summary>
        Task<GraphQLDataDetailedResult<dynamic>> ExecuteDetailed();
    }

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Generated Query which supports executing the query
    /// </summary>
    /// <typeparam name="T">The type of the query</typeparam>
    public interface IGraphQLQuery<T>
        where T : class
    {
        /// <summary>
        /// Execute the query and return the result
        /// </summary>
        /// <returns>The result of the query</returns>
        Task<T> Execute();

        /// <summary>
        /// Execute query and return the result with response headers
        /// </summary>
        /// <returns>Object containing query result and response headers</returns>
        Task<GraphQLDataDetailedResult<T>> ExecuteDetailed();
    }
}