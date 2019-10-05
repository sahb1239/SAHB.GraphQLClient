using System;
using System.Collections.Generic;
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
        /// HttpClient used to send all requests
        /// </summary>
        HttpClient Client { get; }

        /// <summary>
        /// Default method used for all request if not set
        /// </summary>
        HttpMethod DefaultMethod { get; set; }

        /// <summary>
        /// Execute the specified GraphQL query
        /// </summary>
        /// <param name="query">The GraphQL query which should be executed</param>
        /// <param name="url">Url to the GraphQL endpoint</param>
        /// <param name="method">HttpMethod which should be used for the GraphQL endpoint</param>
        /// <param name="authorizationToken">The authorization token which should be used</param>
        /// <param name="authorizationMethod">Authorization method used for the authorization token</param>
        /// <returns></returns>
        Task<GraphQLExecutorResponse> ExecuteQuery(string query, string url = null, HttpMethod method = null, string authorizationToken = null, string authorizationMethod = "Bearer", IDictionary<string, string> headers = null);
    }
}
