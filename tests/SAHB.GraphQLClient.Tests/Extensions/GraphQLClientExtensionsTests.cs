using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.Extensions
{
    public class GraphQLClientExtensionsTests
    {
        public class QueryType
        {
            public string Hello { get; set; }
        }

        public class QueryT
        {
            [Fact]
            public void ShouldThrowArgumentNullExceptionIfClientIsNull()
            {
                // Arrange
                IGraphQLClient client = null;

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.Query<QueryType>());
            }

            [Fact]
            public void ShouldCallCreateHttpRequestTWithOperationType()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLHttpRequest<QueryType>>(x => x.Strict());

                A.CallTo(() => client.CreateHttpRequest<QueryType>(A<GraphQLOperationType>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Url).Returns("some url");

                // Act
                var actual = client.Query<QueryType>();

                // Assert
                Assert.Equal("some url", actual.Url);
                A.CallTo(() => client.CreateHttpRequest<QueryType>(GraphQLOperationType.Query))
                    .MustHaveHappenedOnceExactly();
            }
        }

        public class MutateT
        {
            [Fact]
            public void ShouldThrowArgumentNullExceptionIfClientIsNull()
            {
                // Arrange
                IGraphQLClient client = null;

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.Mutate<QueryType>());
            }

            [Fact]
            public void ShouldCallCreateHttpRequestTWithOperationType()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLHttpRequest<QueryType>>(x => x.Strict());

                A.CallTo(() => client.CreateHttpRequest<QueryType>(A<GraphQLOperationType>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Url).Returns("some url");

                // Act
                var actual = client.Mutate<QueryType>();

                // Assert
                Assert.Equal("some url", actual.Url);
                A.CallTo(() => client.CreateHttpRequest<QueryType>(GraphQLOperationType.Mutation))
                    .MustHaveHappenedOnceExactly();
            }
        }

        public class SubscribeT
        {
            [Fact]
            public void ShouldThrowArgumentNullExceptionIfClientIsNull()
            {
                // Arrange
                IGraphQLClient client = null;

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.Subscribe<QueryType>());
            }

            [Fact]
            public void ShouldCallCreateHttpRequestTWithOperationType()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLSubscriptionRequest<QueryType>>(x => x.Strict());

                A.CallTo(() => client.CreateSubscriptionRequest<QueryType>(A<GraphQLOperationType>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Url).Returns("some url");

                // Act
                var actual = client.Subscribe<QueryType>();

                // Assert
                Assert.Equal("some url", actual.Url);
                A.CallTo(() => client.CreateSubscriptionRequest<QueryType>(GraphQLOperationType.Subscription))
                    .MustHaveHappenedOnceExactly();
            }
        }

        public class QueryQueryBuilder
        {
            [Fact]
            public void ShouldThrowArgumentNullExceptionIfClientIsNull()
            {
                // Arrange
                IGraphQLClient client = null;
                Action<IGraphQLBuilder> queryBuilder = builder => { };

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.Query(queryBuilder));
            }

            [Fact]
            public void ShouldThrowArgumentNullExceptionIfBuilderIsNull()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                Action<IGraphQLBuilder> queryBuilder = null;

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.Query(queryBuilder));
            }

            [Fact]
            public void ShouldCallCreateHttpRequestTWithOperationTypeAndQueryBuilder()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLHttpRequest<dynamic>>(x => x.Strict());
                Action<IGraphQLBuilder> queryBuilder = builder => { };

                A.CallTo(() => client.CreateHttpRequest(A<GraphQLOperationType>.Ignored, A<Action<IGraphQLBuilder>>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Url).Returns("some url");

                // Act
                var actual = client.Query(queryBuilder);

                // Assert
                Assert.Equal("some url", actual.Url);
                A.CallTo(() => client.CreateHttpRequest(GraphQLOperationType.Query, queryBuilder))
                    .MustHaveHappenedOnceExactly();
            }
        }

        public class MutateQueryBuilder
        {
            [Fact]
            public void ShouldThrowArgumentNullExceptionIfClientIsNull()
            {
                // Arrange
                IGraphQLClient client = null;
                Action<IGraphQLBuilder> queryBuilder = builder => { };

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.Mutate(queryBuilder));
            }

            [Fact]
            public void ShouldThrowArgumentNullExceptionIfBuilderIsNull()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                Action<IGraphQLBuilder> queryBuilder = null;

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.Mutate(queryBuilder));
            }

            [Fact]
            public void ShouldCallCreateHttpRequestTWithOperationTypeAndQueryBuilder()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLHttpRequest<dynamic>>(x => x.Strict());
                Action<IGraphQLBuilder> queryBuilder = builder => { };

                A.CallTo(() => client.CreateHttpRequest(A<GraphQLOperationType>.Ignored, A<Action<IGraphQLBuilder>>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Url).Returns("some url");

                // Act
                var actual = client.Mutate(queryBuilder);

                // Assert
                Assert.Equal("some url", actual.Url);
                A.CallTo(() => client.CreateHttpRequest(GraphQLOperationType.Mutation, queryBuilder))
                    .MustHaveHappenedOnceExactly();
            }
        }

        public class SubscribeQueryBuilder
        {
            [Fact]
            public void ShouldThrowArgumentNullExceptionIfClientIsNull()
            {
                // Arrange
                IGraphQLClient client = null;
                Action<IGraphQLBuilder> queryBuilder = builder => { };

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.Subscribe(queryBuilder));
            }

            [Fact]
            public void ShouldThrowArgumentNullExceptionIfBuilderIsNull()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                Action<IGraphQLBuilder> queryBuilder = null;

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.Subscribe(queryBuilder));
            }

            [Fact]
            public void ShouldCallCreateHttpRequestTWithOperationTypeAndQueryBuilder()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLSubscriptionRequest<dynamic>>(x => x.Strict());
                Action<IGraphQLBuilder> queryBuilder = builder => { };

                A.CallTo(() => client.CreateSubscriptionRequest(A<GraphQLOperationType>.Ignored, A<Action<IGraphQLBuilder>>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Url).Returns("some url");

                // Act
                var actual = client.Subscribe(queryBuilder);

                // Assert
                Assert.Equal("some url", actual.Url);
                A.CallTo(() => client.CreateSubscriptionRequest(GraphQLOperationType.Subscription, queryBuilder))
                    .MustHaveHappenedOnceExactly();
            }
        }

        public class QueryBatch
        {
            [Fact]
            public void ShouldThrowArgumentNullExceptionIfClientIsNull()
            {
                // Arrange
                IGraphQLClient client = null;

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.QueryBatch());
            }

            [Fact]
            public void ShouldCallCreateHttpRequestTWithOperationType()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLBatchRequest>(x => x.Strict());

                A.CallTo(() => client.CreateBatchRequest(A<GraphQLOperationType>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Url).Returns("some url");

                // Act
                var actual = client.QueryBatch();

                // Assert
                Assert.Equal("some url", actual.Url);
                A.CallTo(() => client.CreateBatchRequest(GraphQLOperationType.Query))
                    .MustHaveHappenedOnceExactly();
            }
        }

        public class MutateBatch
        {
            [Fact]
            public void ShouldThrowArgumentNullExceptionIfClientIsNull()
            {
                // Arrange
                IGraphQLClient client = null;

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.MutateBatch());
            }

            [Fact]
            public void ShouldCallCreateHttpRequestTWithOperationType()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLBatchRequest>(x => x.Strict());

                A.CallTo(() => client.CreateBatchRequest(A<GraphQLOperationType>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Url).Returns("some url");

                // Act
                var actual = client.MutateBatch();

                // Assert
                Assert.Equal("some url", actual.Url);
                A.CallTo(() => client.CreateBatchRequest(GraphQLOperationType.Mutation))
                    .MustHaveHappenedOnceExactly();
            }
        }

        public class SubscribeBatch
        {
            [Fact]
            public void ShouldThrowArgumentNullExceptionIfClientIsNull()
            {
                // Arrange
                IGraphQLClient client = null;

                // Act / Assert
                Assert.Throws<ArgumentNullException>(() => client.SubscribeBatch());
            }

            [Fact]
            public void ShouldCallCreateHttpRequestTWithOperationType()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLBatchRequest>(x => x.Strict());

                A.CallTo(() => client.CreateBatchRequest(A<GraphQLOperationType>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Url).Returns("some url");

                // Act
                var actual = client.SubscribeBatch();

                // Assert
                Assert.Equal("some url", actual.Url);
                A.CallTo(() => client.CreateBatchRequest(GraphQLOperationType.Subscription))
                    .MustHaveHappenedOnceExactly();
            }
        }

        public class GetIntrospectionSchema
        {
            [Fact]
            public async Task ShouldThrowArgumentNullExceptionIfClientIsNull()
            {
                // Arrange
                IGraphQLClient client = null;

                // Act / Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetIntrospectionSchema());
            }

            [Fact]
            public async Task ShouldCallCreateHttpRequestTWithOperationType()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLHttpRequest<GraphQLIntrospectionQuery>>(x => x.Strict());
                var response = A.Fake<IGraphQLHttpResponse<GraphQLIntrospectionQuery, GraphQLIntrospectionQuery, 
                    IGraphQLHttpRequest<GraphQLIntrospectionQuery>>>(x => x.Strict());

                var expected = new GraphQLIntrospectionQuery()
                {
                    Schema = new GraphQLIntrospectionSchema
                    {

                    }
                };

                A.CallTo(() => client.CreateHttpRequest<GraphQLIntrospectionQuery>(A<GraphQLOperationType>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Execute(A<CancellationToken>.Ignored)).Returns(Task.FromResult(response));
                A.CallToSet(() => request.ShouldThrowIfQueryIsInvalid).DoesNothing();
                A.CallTo(() => response.Data).Returns(expected);

                // Act
                var actual = await client.GetIntrospectionSchema();

                // Assert
                Assert.NotNull(actual);
                Assert.Equal(expected.Schema, actual);

                A.CallTo(() => client.CreateHttpRequest<GraphQLIntrospectionQuery>(GraphQLOperationType.Query))
                    .MustHaveHappenedOnceExactly();

                A.CallToSet(() => request.ShouldThrowIfQueryIsInvalid)
                    .To(false)
                    .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public async Task ShouldCallExecuteOnRequest()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLHttpRequest<GraphQLIntrospectionQuery>>(x => x.Strict());
                var response = A.Fake<IGraphQLHttpResponse<GraphQLIntrospectionQuery, GraphQLIntrospectionQuery,
                    IGraphQLHttpRequest<GraphQLIntrospectionQuery>>>(x => x.Strict());

                var expected = new GraphQLIntrospectionQuery()
                {
                    Schema = new GraphQLIntrospectionSchema
                    {

                    }
                };

                A.CallTo(() => client.CreateHttpRequest<GraphQLIntrospectionQuery>(A<GraphQLOperationType>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Execute(A<CancellationToken>.Ignored)).Returns(Task.FromResult(response));
                A.CallToSet(() => request.ShouldThrowIfQueryIsInvalid).DoesNothing();
                A.CallTo(() => response.Data).Returns(expected);

                // Act
                var actual = await client.GetIntrospectionSchema();

                // Assert
                Assert.NotNull(actual);
                Assert.Equal(expected.Schema, actual);

                A.CallTo(() => request.Execute(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();

                A.CallToSet(() => request.ShouldThrowIfQueryIsInvalid)
                    .To(false)
                    .MustHaveHappened(1, Times.Exactly);
            }

            [Fact]
            public async Task ShouldCallGetData()
            {
                // Arrange
                var client = A.Fake<IGraphQLClient>(x => x.Strict());
                var request = A.Fake<IGraphQLHttpRequest<GraphQLIntrospectionQuery>>(x => x.Strict());
                var response = A.Fake<IGraphQLHttpResponse<GraphQLIntrospectionQuery, GraphQLIntrospectionQuery,
                    IGraphQLHttpRequest<GraphQLIntrospectionQuery>>>(x => x.Strict());

                var expected = new GraphQLIntrospectionQuery()
                {
                    Schema = new GraphQLIntrospectionSchema
                    {

                    }
                };

                A.CallTo(() => client.CreateHttpRequest<GraphQLIntrospectionQuery>(A<GraphQLOperationType>.Ignored))
                    .Returns(request);
                A.CallTo(() => request.Execute(A<CancellationToken>.Ignored)).Returns(Task.FromResult(response));
                A.CallToSet(() => request.ShouldThrowIfQueryIsInvalid).DoesNothing();
                A.CallTo(() => response.Data).Returns(expected);

                // Act
                var actual = await client.GetIntrospectionSchema();

                // Assert
                Assert.NotNull(actual);
                Assert.Equal(expected.Schema, actual);

                A.CallTo(() => response.Data).MustHaveHappenedOnceExactly();

                A.CallToSet(() => request.ShouldThrowIfQueryIsInvalid)
                    .To(false)
                    .MustHaveHappened(1, Times.Exactly);
            }
        }
    }
}
