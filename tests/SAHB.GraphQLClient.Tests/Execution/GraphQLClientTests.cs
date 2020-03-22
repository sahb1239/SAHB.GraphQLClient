using System;
using FakeItEasy;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Execution;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.Execution
{
    public class GraphQLExecutorTests
    {
        [Fact]
        public void InitilizeProperties()
        {
            // Arrange
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>(x => x.Strict());
            var deserialization = A.Fake<IGraphQLDeserialization>(x => x.Strict());
            var httpExecutor = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            var subscriptionExecutor = A.Fake<IGraphQLSubscriptionExecutor>(x => x.Strict());

            // Act
            var executor = new GraphQLExecutor(queryGenerator,
                deserialization, httpExecutor, subscriptionExecutor);

            // Assert
            Assert.Equal(queryGenerator, executor.QueryGenerator);
            Assert.Equal(deserialization, executor.Deserialization);
            Assert.Equal(httpExecutor, executor.HttpExecutor);
            Assert.Equal(subscriptionExecutor, executor.SubscriptionExecutor);
        }

        [Fact]
        public void Throws_If_QueryGenerator_IsNull()
        {
            // Arrange
            IGraphQLQueryGeneratorFromFields queryGenerator = null;
            var deserialization = A.Fake<IGraphQLDeserialization>(x => x.Strict());
            var httpExecutor = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            var subscriptionExecutor = A.Fake<IGraphQLSubscriptionExecutor>(x => x.Strict());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new GraphQLExecutor(queryGenerator,
                    deserialization, httpExecutor, subscriptionExecutor));

            Assert.Equal("queryGenerator", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: queryGenerator", exception.Message);
        }

        [Fact]
        public void Throws_If_Deserialization_IsNull()
        {
            // Arrange
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>(x => x.Strict());
            IGraphQLDeserialization deserialization = null;
            var httpExecutor = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            var subscriptionExecutor = A.Fake<IGraphQLSubscriptionExecutor>(x => x.Strict());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new GraphQLExecutor(queryGenerator,
                    deserialization, httpExecutor, subscriptionExecutor));

            Assert.Equal("deserialization", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: deserialization", exception.Message);
        }

        [Fact]
        public void Throws_If_HttpExecutor_IsNull()
        {
            // Arrange
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>(x => x.Strict());
            var deserialization = A.Fake<IGraphQLDeserialization>(x => x.Strict());
            IGraphQLHttpExecutor httpExecutor = null;
            var subscriptionExecutor = A.Fake<IGraphQLSubscriptionExecutor>(x => x.Strict());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new GraphQLExecutor(queryGenerator,
                    deserialization, httpExecutor, subscriptionExecutor));

            Assert.Equal("httpExecutor", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: httpExecutor", exception.Message);
        }

        [Fact]
        public void Throws_If_SubscriptionExecutor_IsNull()
        {
            // Arrange
            var queryGenerator = A.Fake<IGraphQLQueryGeneratorFromFields>(x => x.Strict());
            var deserialization = A.Fake<IGraphQLDeserialization>(x => x.Strict());
            var httpExecutor = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            IGraphQLSubscriptionExecutor subscriptionExecutor = null;

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new GraphQLExecutor(queryGenerator,
                    deserialization, httpExecutor, subscriptionExecutor));

            Assert.Equal("subscriptionExecutor", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: subscriptionExecutor", exception.Message);
        }
    }
}
