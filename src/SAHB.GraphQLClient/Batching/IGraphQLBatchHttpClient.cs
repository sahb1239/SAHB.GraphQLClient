using System;
using System.Net.Http;

namespace SAHB.GraphQLClient.Batching
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL client which supports generating GraphQL queries and mutations from multiple <see cref="Type"/>s and executing them in batches
    /// </summary>
    public interface IGraphQLBatchHttpClient
    {
        /// <summary>
        /// Generates a GraphQL batch using the specified server <paramref name="url"/>, the specified <paramref name="authorizationToken"/> and the specified <paramref name="authorizationMethod"/>
        /// Default HttpMethod is POST
        /// </summary>
        /// <param name="url">The endpoint to request the GraphQL server from</param>
        /// <param name="authorizationToken">The token used to authentificate with the GraphQL server</param>
        /// <param name="authorizationMethod">The method used for authentification</param>
        /// <returns></returns>
        IGraphQLBatch CreateBatch(string url, string authorizationToken = null, string authorizationMethod = "Bearer");

        /// <summary>
        /// Generates a GraphQL batch using the specified server <paramref name="url"/>, the specified <paramref name="authorizationToken"/> and the specified <paramref name="authorizationMethod"/>
        /// </summary>
        /// <param name="url">The endpoint to request the GraphQL server from</param>
        /// <param name="httpMethod">The HttpMethod to use to communicate with the server, for example POST</param>
        /// <param name="authorizationToken">The token used to authentificate with the GraphQL server</param>
        /// <param name="authorizationMethod">The method used for authentification</param>
        /// <returns></returns>
        IGraphQLBatch CreateBatch(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer");
    }
}