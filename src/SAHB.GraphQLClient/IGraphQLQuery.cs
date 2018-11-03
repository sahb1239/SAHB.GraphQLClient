using SAHB.GraphQLClient.QueryGenerator;
using System.Threading.Tasks;

namespace SAHB.GraphQLClient
{
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
    }
}