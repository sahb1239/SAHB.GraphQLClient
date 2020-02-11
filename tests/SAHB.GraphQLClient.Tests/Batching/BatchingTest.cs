using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;
using System.Net.Http;
using SAHB.GraphQLClient.Executor;
using FakeItEasy;
using System.Linq;
using System.Threading;
using SAHB.GraphQLClient.Filtering;

namespace SAHB.GraphQLClient.Tests.Batching
{
    public class BatchingTest
    {
        [Fact]
        public async Task Test_Combine_Two_Simple_Queries()
        {
            // Arrange
            var expected =
                "{\"query\":\"query{batch0_Part1Field1:part1_field1 batch0_Part1Field2:part1Field2 batch1_Part2Field3:part2_field3 batch1_Part2Field4:part2Field4}\"}";

            var mock = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            A.CallTo(() => mock.ExecuteQuery(expected,
                A<string>.Ignored,
                A<HttpMethod>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<IDictionary<string, string>>.Ignored,
                A<CancellationToken>.Ignored))
                .Returns(new GraphQLExecutorResponse
                {
                    Response = JsonConvert.SerializeObject(new
                    {
                        Data = new
                        {
                            batch0_Part1Field1 = "Value1",
                            batch0_Part1Field2 = "Value2",
                            batch1_Part2Field3 = "Value3",
                            batch1_Part2Field4 = "Value4"
                        }
                    })
                });

            var client = new GraphQLHttpClient(mock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization(), new QueryGeneratorFilter());

            // Act
            var batch = client.CreateBatch("");
            var query1 = batch.Query<QueryBatchPart1>();
            var query2 = batch.Query<QueryBatchPart2>();

            var result1 = await query1.Execute();
            var result2 = await query2.Execute();

            // Assert
            Assert.Equal(result1.Part1Field1, "Value1");
            Assert.Equal(result1.Part1Field2, "Value2");

            Assert.Equal(result2.Part2Field3, "Value3");
            Assert.Equal(result2.Part2Field4, "Value4");
        }

        public class QueryBatchPart1
        {
            [GraphQLFieldName("part1_field1")]
            public string Part1Field1 { get; set; }

            public string Part1Field2 { get; set; }
        }

        public class QueryBatchPart2
        {
            [GraphQLFieldName("part2_field3")]
            public string Part2Field3 { get; set; }

            public string Part2Field4 { get; set; }
        }

        [Fact]
        public async Task Test_Combine_With_Arguments()
        {
            // Arrange
            var expected =
                @"{""query"":""query{batch0_Part1Field1:part1_field1(argumentName:\""1\"") batch0_Part1Field2:part1Field2 batch1_Part2Field3:part2_field3(argumentName:\""2\"") batch1_Part2Field4:part2Field4}""}";
            var httpClientMock = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            A.CallTo(() => httpClientMock.ExecuteQuery(expected,
                A<string>.Ignored,
                A<HttpMethod>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<IDictionary<string, string>>.Ignored,
                A<CancellationToken>.Ignored))
                .Returns(new GraphQLExecutorResponse
                {
                    Response = JsonConvert.SerializeObject(new
                    {
                        Data = new
                        {
                            batch0_Part1Field1 = "Value1",
                            batch0_Part1Field2 = "Value2",
                            batch1_Part2Field3 = "Value3",
                            batch1_Part2Field4 = "Value4"
                        }
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization(), new QueryGeneratorFilter());

            // Act
            var batch = client.CreateBatch("");
            var query1 =
                batch.Query<QueryBatchWithArgumentsPart1>(new GraphQLQueryArgument("argumentVariable", 1.ToString()));
            var query2 =
                batch.Query<QueryBatchWithArgumentsPart2>(new GraphQLQueryArgument("argumentVariable2", 2.ToString()));

            var result1 = await query1.Execute();
            var result2 = await query2.Execute();

            // Assert
            Assert.Equal(result1.Part1Field1, "Value1");
            Assert.Equal(result1.Part1Field2, "Value2");

            Assert.Equal(result2.Part2Field3, "Value3");
            Assert.Equal(result2.Part2Field4, "Value4");
        }

        public class QueryBatchWithArgumentsPart1
        {
            [GraphQLFieldName("part1_field1"), GraphQLArguments("argumentName", "Int", "argumentVariable")]
            public string Part1Field1 { get; set; }

            public string Part1Field2 { get; set; }
        }

        public class QueryBatchWithArgumentsPart2
        {
            [GraphQLFieldName("part2_field3"), GraphQLArguments("argumentName", "Int", "argumentVariable2")]
            public string Part2Field3 { get; set; }

            public string Part2Field4 { get; set; }
        }

        [Fact]
        public async Task Test_Combine_With_Conflicting_Arguments()
        {
            // Arrange
            var expected =
                @"{""query"":""query{batch0_Part1Field1:part1_field1(argumentName:\""1\"") batch0_Part1Field2:part1Field2 batch1_Part2Field3:part2_field3(argumentName:\""2\"") batch1_Part2Field4:part2Field4}""}";
            var httpClientMock = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            A.CallTo(() => httpClientMock.ExecuteQuery(expected,
                A<string>.Ignored,
                A<HttpMethod>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<IDictionary<string, string>>.Ignored,
                A<CancellationToken>.Ignored))
                .Returns(new GraphQLExecutorResponse
                {
                    Response = JsonConvert.SerializeObject(new
                    {
                        Data = new
                        {
                            batch0_Part1Field1 = "Value1",
                            batch0_Part1Field2 = "Value2",
                            batch1_Part2Field3 = "Value3",
                            batch1_Part2Field4 = "Value4"
                        }
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization(), new QueryGeneratorFilter());

            // Act
            var batch = client.CreateBatch("");
            var query1 =
                batch.Query<QueryBatchWithConflictingArgumentsPart1>(new GraphQLQueryArgument("argumentVariable", 1.ToString()));
            var query2 =
                batch.Query<QueryBatchWithConflictingArgumentsPart2>(new GraphQLQueryArgument("argumentVariable", 2.ToString()));

            var result1 = await query1.Execute();
            var result2 = await query2.Execute();

            // Assert
            Assert.Equal(result1.Part1Field1, "Value1");
            Assert.Equal(result1.Part1Field2, "Value2");

            Assert.Equal(result2.Part2Field3, "Value3");
            Assert.Equal(result2.Part2Field4, "Value4");
        }

        public class QueryBatchWithConflictingArgumentsPart1
        {
            [GraphQLFieldName("part1_field1"), GraphQLArguments("argumentName", "Int", "argumentVariable")]
            public string Part1Field1 { get; set; }

            public string Part1Field2 { get; set; }
        }

        public class QueryBatchWithConflictingArgumentsPart2
        {
            [GraphQLFieldName("part2_field3"), GraphQLArguments("argumentName", "Int", "argumentVariable")]
            public string Part2Field3 { get; set; }

            public string Part2Field4 { get; set; }
        }

        [Fact]
        public async Task Test_Execute_Should_Set_IsExecuted()
        {
            // Arrange
            var expected =
                @"{""query"":""query{batch0_Part1Field1:part1_field1(argumentName:\""1\"") batch0_Part1Field2:part1Field2}""}";

            var httpClientMock = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            A.CallTo(() => httpClientMock.ExecuteQuery(expected,
                A<string>.Ignored,
                A<HttpMethod>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<IDictionary<string, string>>.Ignored,
                A<CancellationToken>.Ignored))
                .Returns(new GraphQLExecutorResponse
                {
                    Response = JsonConvert.SerializeObject(new
                    {
                        Data = new
                        {
                            batch0_Part1Field1 = "Value1",
                            batch0_Part1Field2 = "Value2"
                        }
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization(), new QueryGeneratorFilter());

            // Act
            var batch = client.CreateBatch("");
            var query1 =
                batch.Query<QueryBatchWithConflictingArgumentsPart1>(new GraphQLQueryArgument("argumentVariable", 1.ToString()));
            var result1 = await query1.Execute();

            // Assert isExecuted
            Assert.True(batch.IsExecuted());
        }

        [Fact]
        public async Task Test_Execute_And_Add_After_Execute_Should_Throw_Exception()
        {
            // Arrange
            var expected =
                @"{""query"":""query{batch0_Part1Field1:part1_field1(argumentName:\""1\"") batch0_Part1Field2:part1Field2}""}";
            var httpClientMock = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            A.CallTo(() => httpClientMock.ExecuteQuery(expected,
                A<string>.Ignored,
                A<HttpMethod>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<IDictionary<string, string>>.Ignored,
                A<CancellationToken>.Ignored))
                .Returns(new GraphQLExecutorResponse
                {
                    Response = JsonConvert.SerializeObject(new
                    {
                        Data = new
                        {
                            batch0_Part1Field1 = "Value1",
                            batch0_Part1Field2 = "Value2"
                        }
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization(), new QueryGeneratorFilter());

            // Act
            var batch = client.CreateBatch("");
            var query1 =
                batch.Query<QueryBatchWithConflictingArgumentsPart1>(new GraphQLQueryArgument("argumentVariable", 1.ToString()));
            var result1 = await query1.Execute();

            // Assert Query should throw
            // Get query2
            Assert.Throws<GraphQLBatchAlreadyExecutedException>(() =>
            {
                batch.Query<QueryBatchWithConflictingArgumentsPart2>(
                    new GraphQLQueryArgument("argumentVariable", 2.ToString()));
            });
        }

        [Fact]
        public async Task Test_Execute_Two_Times_Should_Not_Throw_Exception()
        {
            // Arrange
            var expected =
                @"{""query"":""query{batch0_Part1Field1:part1_field1(argumentName:\""1\"") batch0_Part1Field2:part1Field2}""}";

            var httpClientMock = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            A.CallTo(() => httpClientMock.ExecuteQuery(expected,
                A<string>.Ignored,
                A<HttpMethod>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<IDictionary<string, string>>.Ignored,
                A<CancellationToken>.Ignored))
                .Returns(new GraphQLExecutorResponse
                {
                    Response = JsonConvert.SerializeObject(new
                    {
                        Data = new
                        {
                            batch0_Part1Field1 = "Value1",
                            batch0_Part1Field2 = "Value2"
                        }
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization(), new QueryGeneratorFilter());

            // Act
            var batch = client.CreateBatch("");
            var query1 =
                batch.Query<QueryBatchWithConflictingArgumentsPart1>(new GraphQLQueryArgument("argumentVariable", 1.ToString()));
            var result1 = await query1.Execute();

            //  Query should not throw but instead just return same output
            var result2 = await query1.Execute();

            // Assert
            Assert.Equal(result1.Part1Field1, result2.Part1Field1);
            Assert.Equal(result1.Part1Field2, result2.Part1Field2);
        }

        [Fact]
        public async Task Test_ExecuteDetailed_Returns_Expected_Headers_And_Data()
        {
            // Arrange
            var expectedQuery =
                "{\"query\":\"query{batch0_Part1Field1:part1_field1 batch0_Part1Field2:part1Field2 batch1_Part2Field3:part2_field3 batch1_Part2Field4:part2Field4}\"}";
            var responseHeaders = new HttpResponseMessage().Headers;
            responseHeaders.Add("TestHeader", "TestValue");

            var httpClientMock = A.Fake<IGraphQLHttpExecutor>(x => x.Strict());
            A.CallTo(() => httpClientMock.ExecuteQuery(expectedQuery,
                A<string>.Ignored,
                A<HttpMethod>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<IDictionary<string, string>>.Ignored,
                A<CancellationToken>.Ignored))
                .Returns(new GraphQLExecutorResponse
                {
                    Response = JsonConvert.SerializeObject(new
                    {
                        Data = new
                        {
                            batch0_Part1Field1 = "Value1",
                            batch0_Part1Field2 = "Value2",
                            batch1_Part2Field3 = "Value3",
                            batch1_Part2Field4 = "Value4"
                        }
                    }),
                    Headers = responseHeaders
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization(), new QueryGeneratorFilter());

            // Act
            var batch = client.CreateBatch("");
            var query1 = batch.Query<QueryBatchPart1>();
            var query2 = batch.Query<QueryBatchPart2>();

            var result1 = await query1.ExecuteDetailed();
            var result2 = await query2.ExecuteDetailed();

            // Assert
            Assert.Equal(result1.Data.Part1Field1, "Value1");
            Assert.Equal(result1.Data.Part1Field2, "Value2");

            Assert.Equal(result2.Data.Part2Field3, "Value3");
            Assert.Equal(result2.Data.Part2Field4, "Value4");

            IEnumerable<string> expectedHeaders = responseHeaders.GetValues("TestHeader");
            IEnumerable<string> actualHeaders = result1.Headers.GetValues("TestHeader");
            Assert.Equal(actualHeaders, expectedHeaders);

            responseHeaders.TryGetValues("TestHeader", out expectedHeaders);
            result2.Headers.TryGetValues("TestHeader", out actualHeaders);
            Assert.Equal(actualHeaders, expectedHeaders);
        }
    }
}
