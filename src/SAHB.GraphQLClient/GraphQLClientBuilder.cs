using Microsoft.Extensions.DependencyInjection;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Http;
using SAHB.GraphQLClient.QueryBuilder;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Extention methods for <see cref="GraphQLClient"/> for registrering in a <see cref="IServiceCollection"/> for the libary Microsoft.Extensions.DependencyInjection
    /// </summary>
    public static class GraphQLClientBuilder
    {
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Registrer the default <see cref="IGraphQLFieldBuilder"/>, <see cref="IGraphQLQueryBuilder"/>, <see cref="IGraphQLClient"/> and the default <see cref="IHttpClient"/> in the specified <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services">The service collection to registrer in</param>
        /// <returns>Returns the service collection</returns>
        public static IServiceCollection AddGraphQLClient(this IServiceCollection services)
        {
            // GraphQL
            services.AddSingleton<IGraphQLFieldBuilder, GraphQLFieldBuilder>();
            services.AddSingleton<IGraphQLQueryBuilder, GraphQLQueryBuilder>();
            services.AddSingleton<IGraphQLClient, GraphQLClient>();
            services.AddSingleton<IHttpClient, HttpClient>();
            return services;
        }
    }
}
