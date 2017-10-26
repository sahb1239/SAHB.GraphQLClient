using System;
using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.QueryBuilder;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL client which supports generating GraphQL queries and mutations from a <see cref="Type"/>
    /// </summary>
    [Obsolete]
    public interface IGraphQLClient
    {
        /// <summary>
        /// Sends a query to a GraphQL server using a specified type, the specified URL and the HttpMethod Post
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="url">The url to request the GraphQL server from using HTTP Post</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The data returned from the query</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        Task<T> Query<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Sends a mutation to a GraphQL server using a specified type, the specified URL and the HttpMethod Post
        /// </summary>
        /// <typeparam name="T">The type to generate the mutation from</typeparam>
        /// <param name="url">The url to request the GraphQL server from using HTTP Post</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The data returned from the query</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        Task<T> Mutate<T>(string url, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Sends a query to a GraphQL server using a specified type, the specified URL and the HttpMethod
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="url">The url to request the GraphQL server</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The data returned from the query</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        Task<T> Query<T>(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;

        /// <summary>
        /// Sends a mutation to a GraphQL server using a specified type, the specified URL and the HttpMethod
        /// </summary>
        /// <typeparam name="T">The type to generate the mutation from</typeparam>
        /// <param name="url">The url to request the GraphQL server</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The data returned from the query</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        Task<T> Mutate<T>(string url, HttpMethod httpMethod, string authorizationToken = null, string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class;
    }
}