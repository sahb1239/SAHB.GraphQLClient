using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Executor
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL executor which executes a query against a http GraphQL server
    /// </summary>
    public interface IGraphQLHttpExecutor
    {
        /// <summary>
        /// Execute the specified GraphQL query
        /// </summary>
        /// <typeparam name="T">The retun type in the data property</typeparam>
        /// <param name="query">The GraphQL query which should be executed</param>
        /// <param name="url">Url to the GraphQL endpoint</param>
        /// <param name="method">HttpMethod which should be used for the GraphQL endpoint</param>
        /// <param name="authorizationToken">The authorization token which should be used</param>
        /// <param name="authorizationMethod">Authorization method used for the authorization token</param>
        /// <returns></returns>
        Task<GraphQLDataResult<T>> ExecuteQuery<T>(string query, string url, HttpMethod method, string authorizationToken = null, string authorizationMethod = "Bearer") where T : class;
    }
}