using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using SAHB.GraphQLClient;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.Extensions
{
    public class GraphQLHttpRequestExtensionsTests
    {
        public class QueryType
        {
            public string Hello { get; set; }
        }

        public class GetDataTInput
        {
            [Fact]
            public async Task ShouldThrowArgumentNullExceptionIfRequestIsNull()
            {
                // Arrange
                IGraphQLHttpRequest<QueryType> request = null;

                // Act / Assert
                await Assert.ThrowsAsync<ArgumentNullException>(() => request.GetData());
            }

            [Fact]
            public async Task ShouldCallExecuteOnRequest()
            {
                // Arrange
                var request = A.Fake<IGraphQLHttpRequest<QueryType>>(x => x.Strict());
                var response = A.Fake<IGraphQLHttpResponse<QueryType, QueryType,
                    IGraphQLHttpRequest<QueryType>>>(x => x.Strict());

                var expected = new QueryType
                {
                    Hello = "hello test"
                };

                A.CallTo(() => request.Execute(A<CancellationToken>.Ignored)).Returns(Task.FromResult(response));
                A.CallTo(() => response.Data).Returns(expected);

                // Act
                var actual = await request.GetData();

                // Assert
                Assert.NotNull(actual);
                Assert.Equal("hello test", actual.Hello);

                A.CallTo(() => request.Execute(A<CancellationToken>.Ignored)).MustHaveHappenedOnceExactly();
            }

            [Fact]
            public async Task ShouldCallGetData()
            {
                // Arrange
                var request = A.Fake<IGraphQLHttpRequest<QueryType>>(x => x.Strict());
                var response = A.Fake<IGraphQLHttpResponse<QueryType, QueryType,
                    IGraphQLHttpRequest<QueryType>>>(x => x.Strict());

                var expected = new QueryType
                {
                    Hello = "hello test"
                };

                A.CallTo(() => request.Execute(A<CancellationToken>.Ignored)).Returns(Task.FromResult(response));
                A.CallTo(() => response.Data).Returns(expected);

                // Act
                var actual = await request.GetData();

                // Assert
                Assert.NotNull(actual);
                Assert.Equal("hello test", actual.Hello);

                A.CallTo(() => response.Data).MustHaveHappenedOnceExactly();
            }
        }
    }
}
