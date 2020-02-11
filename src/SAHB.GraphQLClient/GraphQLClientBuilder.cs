using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Filtering;
using SAHB.GraphQLClient.QueryGenerator;
#if DOTNET_HTTP
using System.Net.Http;
#endif

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Extension methods for <see cref="GraphQLHttpClient"/> for registering in a <see cref="IServiceCollection"/> for the library Microsoft.Extensions.DependencyInjection
    /// </summary>
    public static class GraphQLClientBuilder
    {
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Register the default <see cref="IGraphQLFieldBuilder"/>, <see cref="IGraphQLHttpClient"/> and the default <see cref="IGraphQLHttpExecutor"/> in the specified <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services">The service collection to register in</param>
        /// <returns>Returns the service collection</returns>
        public static IServiceCollection AddGraphQLHttpClient(this IServiceCollection services)
        {
            // GraphQL
            services.AddSingleton<IGraphQLFieldBuilder>(provider =>
                new GraphQLFieldBuilder() { LoggerFactory = provider.GetService<ILoggerFactory>() });
            services.AddSingleton<IGraphQLQueryGeneratorFromFields>(provider =>
                new GraphQLQueryGeneratorFromFields() { LoggerFactory = provider.GetService<ILoggerFactory>() });
            services.AddSingleton<IGraphQLDeserialization, GraphQLDeserilization>();
            services.AddSingleton<IQueryGeneratorFilter, QueryGeneratorFilter>();

#if DOTNET_HTTP
            services.AddHttpClient();
            services.AddScoped<IGraphQLHttpExecutor>(provider =>
               new GraphQLHttpExecutor(provider.GetRequiredService<IHttpClientFactory>().CreateClient())
               {
                   LoggerFactory = provider.GetService<ILoggerFactory>()
               });
#else
            services.AddScoped<IGraphQLHttpExecutor>(provider =>
               new GraphQLHttpExecutor()
               {
                   LoggerFactory = provider.GetService<ILoggerFactory>()
               });
#endif

            services.AddScoped<IGraphQLHttpClient>(provider =>
                new GraphQLHttpClient(provider.GetRequiredService<IGraphQLHttpExecutor>(),
                    provider.GetRequiredService<IGraphQLFieldBuilder>(),
                    provider.GetRequiredService<IGraphQLQueryGeneratorFromFields>(),
                    provider.GetRequiredService<IGraphQLDeserialization>(),
                    provider.GetRequiredService<IQueryGeneratorFilter>())
                {
                    LoggerFactory = provider.GetService<ILoggerFactory>()
                });
            return services;
        }
    }
}
