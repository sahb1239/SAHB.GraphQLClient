using Microsoft.Extensions.DependencyInjection;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Http;
using SAHB.GraphQLClient.QueryBuilder;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    public static class GraphQLClientBuilder
    {
        // ReSharper disable once InconsistentNaming
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
