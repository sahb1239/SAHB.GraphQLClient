using System.Threading;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Generated Query which supports executing the query
    /// </summary>
    public interface IGraphQLQuery
    {
        /// <summary>
        /// Execute the query
        /// </summary>
        /// <param name="cancellationToken">A token that signals that the caller requested cancellation of this method invocation</param>
        /// <returns>The result of the query</returns>
        Task<dynamic> Execute(CancellationToken cancellationToken = default);

        /// <summary>
        /// Execute query and return the result with response headers
        /// </summary>
        /// <param name="cancellationToken">A token that signals that the caller requested cancellation of this method invocation</param>
        /// <returns>Object containing query result and response headers</returns>
        Task<GraphQLDataResult<dynamic>> ExecuteDetailed(CancellationToken cancellationToken = default);
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
        /// <param name="cancellationToken">A token that signals that the caller requested cancellation of this method invocation</param>
        /// <returns>The result of the query</returns>
        Task<T> Execute(CancellationToken cancellationToken = default);

        /// <summary>
        /// Execute query and return the result with response headers
        /// </summary>
        /// <param name="cancellationToken">A token that signals that the caller requested cancellation of this method invocation</param>
        /// <returns>Object containing query result and response headers</returns>
        Task<GraphQLDataResult<T>> ExecuteDetailed(CancellationToken cancellationToken = default);
    }
}
