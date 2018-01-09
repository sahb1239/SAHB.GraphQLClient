using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Batching;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.GraphQLClientBuilder
{
    public class ServiceCollectionTest
    {
        [Fact]
        public void Test_GetService_GraphQLHttpClient()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGraphQLHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                Assert.NotNull(scope.ServiceProvider.GetService<IGraphQLHttpClient>());
            }
        }

        [Fact]
        public void Test_GetService_GraphQLHttpExecutor()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGraphQLHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                Assert.NotNull(scope.ServiceProvider.GetService<IGraphQLHttpExecutor>());
            }
        }

        [Fact]
        public void Test_GetService_GraphQLQueryGenerator()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGraphQLHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                Assert.NotNull(scope.ServiceProvider.GetService<IGraphQLQueryGenerator>());
            }
        }

        [Fact]
        public void Test_GetService_GraphQLQueryGeneratorFromFields()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGraphQLHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                Assert.NotNull(scope.ServiceProvider.GetService<IGraphQLQueryGeneratorFromFields>());
            }
        }

        [Fact]
        public void Test_GetService_GraphQLFieldBuilder()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGraphQLHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                Assert.NotNull(scope.ServiceProvider.GetService<IGraphQLFieldBuilder>());
            }
        }
    }
}
