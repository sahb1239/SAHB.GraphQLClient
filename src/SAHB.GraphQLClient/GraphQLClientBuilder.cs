﻿using System;
using Microsoft.Extensions.DependencyInjection;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryBuilder;

namespace SAHB.GraphQLClient
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Extention methods for <see cref="GraphQLHttpClient"/> for registrering in a <see cref="IServiceCollection"/> for the libary Microsoft.Extensions.DependencyInjection
    /// </summary>
    public static class GraphQLClientBuilder
    {
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Registrer the default <see cref="IGraphQLFieldBuilder"/>, <see cref="IGraphQLQueryBuilder"/>, <see cref="IGraphQLHttpClient"/> and the default <see cref="IGraphQLHttpExecutor"/> in the specified <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services">The service collection to registrer in</param>
        /// <returns>Returns the service collection</returns>
        public static IServiceCollection AddGraphQLHttpClient(this IServiceCollection services)
        {
            // GraphQL
            services.AddSingleton<IGraphQLFieldBuilder, GraphQLFieldBuilder>();
            services.AddSingleton<GraphQLQueryBuilder>();
            services.AddSingleton<IGraphQLQueryBuilder>(serviceProvider => serviceProvider.GetService<GraphQLQueryBuilder>());
            services.AddSingleton<IGraphQLQueryBuilderFromFields>(serviceProvider => serviceProvider.GetService<GraphQLQueryBuilder>());
            services.AddSingleton<IGraphQLHttpExecutor, GraphQLHttpExecutor>();
            services.AddSingleton<IGraphQLHttpClient, GraphQLHttpClient>();
            return services;
        }
    }
}
