using System;
using System.Net.Http;
using System.Threading.Tasks;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Extentions
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Contains extention methods for <see cref="IGraphQLHttpClient"/>
    /// </summary>
    public static class GraphQLHttpClientExtentions
    {
        /// <summary>
        /// Sends a query to a GraphQL server using a specified type, the specified URL and the HttpMethod Post
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="client">The IGraphQLHttpClient</param>
        /// <param name="url">The url to request the GraphQL server from using HTTP Post</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The data returned from the query</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        public static Task<T> Query<T>(this IGraphQLHttpClient client, string url, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            var query = client.CreateQuery<T>(url, authorizationToken, authorizationMethod, arguments);
            return query.Execute();
        }

        /// <summary>
        /// Sends a mutation to a GraphQL server using a specified type, the specified URL and the HttpMethod Post
        /// </summary>
        /// <typeparam name="T">The type to generate the mutation from</typeparam>
        /// <param name="client">The IGraphQLHttpClient</param>
        /// <param name="url">The url to request the GraphQL server from using HTTP Post</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The data returned from the query</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        public static Task<T> Mutate<T>(this IGraphQLHttpClient client, string url, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            var query = client.CreateMutation<T>(url, authorizationToken, authorizationMethod, arguments);
            return query.Execute();
        }

        /// <summary>
        /// Sends a query to a GraphQL server using a specified type, the specified URL and the HttpMethod
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="client">The IGraphQLHttpClient</param>
        /// <param name="url">The url to request the GraphQL server</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The data returned from the query</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        public static Task<T> Query<T>(this IGraphQLHttpClient client, string url, HttpMethod httpMethod, string authorizationToken = null,
            string authorizationMethod = "Bearer", params GraphQLQueryArgument[] arguments) where T : class
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            var query = client.CreateQuery<T>(url, httpMethod, authorizationToken, authorizationMethod, arguments);
            return query.Execute();
        }

        /// <summary>
        /// Sends a mutation to a GraphQL server using a specified type, the specified URL and the HttpMethod
        /// </summary>
        /// <typeparam name="T">The type to generate the mutation from</typeparam>
        /// <param name="client">The IGraphQLHttpClient</param>
        /// <param name="url">The url to request the GraphQL server</param>
        /// <param name="httpMethod">The httpMethod to use requesting the GraphQL server</param>
        /// <param name="authorizationToken">Authorization token inserted in the Authorization header</param>
        /// <param name="authorizationMethod">The authorization method inserted in the Authorization header. This is only used when authorizationToken is not null</param>
        /// <param name="arguments">The arguments used in the query which is inserted in the variables</param>
        /// <returns>The data returned from the query</returns>
        /// <exception cref="GraphQLErrorException">Thrown when validation or GraphQL endpoint returns an error</exception>
        public static Task<T> Mutate<T>(this IGraphQLHttpClient client, string url, HttpMethod httpMethod,
            string authorizationToken = null, string authorizationMethod = "Bearer",
            params GraphQLQueryArgument[] arguments) where T : class
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            var query = client.CreateMutation<T>(url, httpMethod, authorizationToken, authorizationMethod, arguments);
            return query.Execute();
        }
    }
}
