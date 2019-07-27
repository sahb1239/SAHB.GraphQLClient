using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;

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
                new GraphQLFieldBuilder() {LoggerFactory = provider.GetService<ILoggerFactory>()});
            services.AddSingleton<IGraphQLQueryGeneratorFromFields>(provider =>
                new GraphQLQueryGeneratorFromFields() { LoggerFactory = provider.GetService<ILoggerFactory>() });
            services.AddSingleton<IGraphQLHttpExecutor>(provider =>
                new GraphQLHttpExecutor() { LoggerFactory = provider.GetService<ILoggerFactory>() });
            services.AddSingleton<IGraphQLHttpClient>(provider =>
                new GraphQLHttpClient(provider.GetRequiredService<IGraphQLHttpExecutor>(),
                    provider.GetRequiredService<IGraphQLFieldBuilder>(),
                    provider.GetRequiredService<IGraphQLQueryGeneratorFromFields>())
                {
                    LoggerFactory = provider.GetService<ILoggerFactory>()
                });
            return services;
        }
    }
}
