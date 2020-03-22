using System;
using System.Collections.Generic;
using FakeItEasy;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.Execution;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Filtering;
using Xunit;

namespace SAHB.GraphQLClient.Tests.GraphQLClient
{
    public class GraphQLClientTests
    {
        [Fact]
        public void InitilizeProperties()
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());
            var executor = A.Fake<IGraphQLExecutor>(x => x.Strict());

            // Act
            var client = new SAHB.GraphQLClient.GraphQLClient(fieldBuilder, 
                filterGenerator, validator, executor);

            // Assert
            Assert.Equal(fieldBuilder, client.FieldBuilder);
            Assert.Equal(filterGenerator, client.FilterGenerator);
            Assert.Equal(validator, client.Validator);
            Assert.Equal(executor, client.Executor);
        }

        [Fact]
        public void Throws_If_FieldBuilder_IsNull()
        {
            // Arrange
            IGraphQLFieldBuilder fieldBuilder = null;
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());
            var executor = A.Fake<IGraphQLExecutor>(x => x.Strict());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() => 
                new SAHB.GraphQLClient.GraphQLClient(fieldBuilder,
                    filterGenerator, validator, executor));

            Assert.Equal("fieldBuilder", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: fieldBuilder", exception.Message);
        }

        [Fact]
        public void Throws_If_FilterGenerator_IsNull()
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            IQueryGeneratorFilter filterGenerator = null;
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());
            var executor = A.Fake<IGraphQLExecutor>(x => x.Strict());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SAHB.GraphQLClient.GraphQLClient(fieldBuilder,
                    filterGenerator, validator, executor));

            Assert.Equal("filterGenerator", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: filterGenerator", exception.Message);
        }

        [Fact]
        public void Throws_If_Validator_IsNull()
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            IGraphQLValidation validator = null;
            var executor = A.Fake<IGraphQLExecutor>(x => x.Strict());

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SAHB.GraphQLClient.GraphQLClient(fieldBuilder,
                    filterGenerator, validator, executor));

            Assert.Equal("validator", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: validator", exception.Message);
        }

        [Fact]
        public void Throws_If_Executor_IsNull()
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());
            IGraphQLExecutor executor = null;

            // Act / Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SAHB.GraphQLClient.GraphQLClient(fieldBuilder,
                    filterGenerator, validator, executor));

            Assert.Equal("executor", exception.ParamName);
            Assert.Equal($"Value cannot be null.{Environment.NewLine}Parameter name: executor", exception.Message);
        }

        [Theory]
        [InlineData(GraphQLOperationType.Query)]
        [InlineData(GraphQLOperationType.Mutation)]
        [InlineData(GraphQLOperationType.Subscription)]
        public void CreateBatchRequest_Returns_BatchRequest(GraphQLOperationType operationType)
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());
            var executor = A.Fake<IGraphQLExecutor>(x => x.Strict());
            var client = new SAHB.GraphQLClient.GraphQLClient(fieldBuilder,
                filterGenerator, validator, executor);

            // Act
            var actual = client.CreateBatchRequest(operationType);

            // Assert
            Assert.IsType<GraphQLBatchRequest>(actual);
            Assert.Equal(operationType, actual.Operation);
            Assert.Equal(client, ((GraphQLRequestInformation)actual).Client);
        }

        [Theory]
        [InlineData(GraphQLOperationType.Query)]
        [InlineData(GraphQLOperationType.Mutation)]
        [InlineData(GraphQLOperationType.Subscription)]
        public void CreateHttpRequestT_Returns_BatchRequest(GraphQLOperationType operationType)
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());
            var executor = A.Fake<IGraphQLExecutor>(x => x.Strict());
            var client = new SAHB.GraphQLClient.GraphQLClient(fieldBuilder,
                filterGenerator, validator, executor);

            var selectionSetResult = new List<GraphQLField>
            {
                new GraphQLField("", "hello", null, null),
            };
            A.CallTo(() => fieldBuilder.GenerateSelectionSet(typeof(QueryType))).Returns(selectionSetResult);

            // Act
            var actual = client.CreateHttpRequest<QueryType>(operationType);

            // Assert
            Assert.IsType<GraphQLHttpRequest<QueryType>>(actual);
            Assert.Equal(operationType, actual.Operation);
            Assert.Equal(client, ((GraphQLRequestInformation)actual).Client);

            Assert.Equal(selectionSetResult, actual.SelectionSet);

            A.CallTo(() => fieldBuilder.GenerateSelectionSet(typeof(QueryType))).MustHaveHappened(1, Times.Exactly);
        }

        [Theory]
        [InlineData(GraphQLOperationType.Query)]
        [InlineData(GraphQLOperationType.Mutation)]
        [InlineData(GraphQLOperationType.Subscription)]
        public void CreateHttpRequest_Returns_BatchRequest(GraphQLOperationType operationType)
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());
            var executor = A.Fake<IGraphQLExecutor>(x => x.Strict());
            var client = new SAHB.GraphQLClient.GraphQLClient(fieldBuilder,
                filterGenerator, validator, executor);
            Action<IGraphQLBuilder> queryBuilder = builder => builder.Field("hello");

            var selectionSetResult = new List<GraphQLField>
            {
                new GraphQLField("", "hello", null, null),
            };

            // Act
            var actual = client.CreateHttpRequest(operationType, queryBuilder);

            // Assert
            Assert.IsType<GraphQLHttpRequest<dynamic>>(actual);
            Assert.Equal(operationType, actual.Operation);
            Assert.Equal(client, ((GraphQLRequestInformation)actual).Client);
        }

        [Theory]
        [InlineData(GraphQLOperationType.Query)]
        [InlineData(GraphQLOperationType.Mutation)]
        [InlineData(GraphQLOperationType.Subscription)]
        public void CreateSubscriptionRequestT_Returns_BatchRequest(GraphQLOperationType operationType)
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());
            var executor = A.Fake<IGraphQLExecutor>(x => x.Strict());
            var client = new SAHB.GraphQLClient.GraphQLClient(fieldBuilder,
                filterGenerator, validator, executor);

            var selectionSetResult = new List<GraphQLField>
            {
                new GraphQLField("", "hello", null, null),
            };
            A.CallTo(() => fieldBuilder.GenerateSelectionSet(typeof(QueryType))).Returns(selectionSetResult);

            // Act
            var actual = client.CreateSubscriptionRequest<QueryType>(operationType);

            // Assert
            Assert.IsType<GraphQLSubscriptionRequest<QueryType>>(actual);
            Assert.Equal(operationType, actual.Operation);
            Assert.Equal(client, ((GraphQLRequestInformation)actual).Client);

            Assert.Equal(selectionSetResult, actual.SelectionSet);

            A.CallTo(() => fieldBuilder.GenerateSelectionSet(typeof(QueryType))).MustHaveHappened(1, Times.Exactly);
        }

        [Theory]
        [InlineData(GraphQLOperationType.Query)]
        [InlineData(GraphQLOperationType.Mutation)]
        [InlineData(GraphQLOperationType.Subscription)]
        public void CreateSubscriptionRequest_Returns_BatchRequest(GraphQLOperationType operationType)
        {
            // Arrange
            var fieldBuilder = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            var filterGenerator = A.Fake<IQueryGeneratorFilter>(x => x.Strict());
            var validator = A.Fake<IGraphQLValidation>(x => x.Strict());
            var executor = A.Fake<IGraphQLExecutor>(x => x.Strict());
            var client = new SAHB.GraphQLClient.GraphQLClient(fieldBuilder,
                filterGenerator, validator, executor);
            Action<IGraphQLBuilder> queryBuilder = builder => builder.Field("hello");

            // Act
            var actual = client.CreateSubscriptionRequest(operationType, queryBuilder);

            // Assert
            Assert.IsType<GraphQLSubscriptionRequest<dynamic>>(actual);
            Assert.Equal(operationType, actual.Operation);
            Assert.Equal(client, ((GraphQLRequestInformation)actual).Client);
        }

        private class QueryType
        {
            public string Hello { get; set; }
        }
    }
}
