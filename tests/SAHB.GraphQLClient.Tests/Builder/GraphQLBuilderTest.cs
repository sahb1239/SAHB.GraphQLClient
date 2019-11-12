using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.Builder
{
    public class GraphQLBuilderTest
    {
        [Fact]
        public async Task Test_GraphQL_Builder_Get_Result()
        {
            var expected = "{\"query\":\"query{alias1:field1 alias2:field2}\"}";
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
                            alias1 = "Value1",
                            alias2 = "Value2"
                        }
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());

            // Act
            var query = client.CreateQuery(builder =>
                builder.Field("field1", field1 => field1.Alias("alias1"))
                    .Field("field2", field2 => field2.Alias("alias2")), "randomurl");
            var result = await query.Execute();

            // Assert
            Assert.Equal("Value1", result["alias1"].Value);
            Assert.Equal("Value2", result["alias2"].Value);
        }

        [Fact]
        public async Task Test_GraphQL_Builder_Empty_Result()
        {
            var expected = "{\"query\":\"query{doSomeAction}\"}";
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
                        Data = (string)null
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());

            // Act
            var query = client.CreateQuery(builder =>
                builder.Field("doSomeAction"), "randomurl");
            var result = await query.Execute();

            // Assert
            Assert.Equal(null, result);
        }

        [Fact]
        public async Task Test_GraphQL_Builder_Argument()
        {
            var expected = "{\"query\":\"query{doSomeAction(argumentName:1)}\"}";
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
                        Data = (string)null
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());

            // Act
            var query = client.CreateQuery(builder =>
                builder.Field("doSomeAction", field =>
                    {
                        field.Argument("argumentName", "argumentType", "variableName");
                    }), "randomurl", arguments: new GraphQLQueryArgument("variableName", 1));
            var result = await query.Execute();

            // Assert
            Assert.Equal(null, result);
        }

        [Fact]
        public async Task Test_GraphQL_Builder_Argument_Implicit_Optional_Does_Not_Throw()
        {
            var expected = "{\"query\":\"query{doSomeAction}\"}";
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
                        Data = (string)null
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());

            // Act
            var query = client.CreateQuery(builder =>
                builder.Field("doSomeAction", field =>
                {
                    field.Argument("argumentName", "argumentType", "variableName");
                }), "randomurl");
            var result = await query.Execute();

            // Assert
            Assert.Equal(null, result);
        }

        [Fact]
        public async Task Test_GraphQL_Builder_Argument_Explicit_Optional_Does_Not_Throw()
        {
            var expected = "{\"query\":\"query{doSomeAction}\"}";
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
                        Data = (string)null
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());

            // Act
            var query = client.CreateQuery(builder =>
                builder.Field("doSomeAction", field =>
                {
                    field.Argument("argumentName", "argumentType", "variableName", isRequired: false);
                }), "randomurl");
            var result = await query.Execute();

            // Assert
            Assert.Equal(null, result);
        }

        [Fact]
        public async Task Test_GraphQL_Builder_Argument_Required_Throws()
        {
            var expected = "{\"query\":\"query{doSomeAction}\"}";
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
                        Data = (string)null
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());

            // Act / Assert
            await Assert.ThrowsAsync<GraphQLArgumentsRequiredException>(() => client.CreateQuery(builder =>
                    builder.Field("doSomeAction",
                        field => { field.Argument("argumentName", "argumentType", "variableName", isRequired: true); }),
                "randomurl").Execute());
        }

        [Fact]
        public async Task Test_GraphQL_Builder_Argument_Inlined_Explicit_Off()
        {
            var expected = "{\"query\":\"query($variableName:argumentType){doSomeAction(argumentName:$variableName)}\",\"variables\":{\"variableName\":1}}";
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
                        Data = (string)null
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());

            // Act
            var query = client.CreateQuery(builder =>
                builder.Field("doSomeAction", field =>
                {
                    field.Argument("argumentName", "argumentType", "variableName", isRequired: true, inlineArgument: false);
                }), "randomurl", arguments: new GraphQLQueryArgument("variableName", 1));
            var result = await query.Execute();

            // Assert
            Assert.Equal(null, result);
        }

        [Fact]
        public async Task Test_GraphQL_Builder_Argument_Inlined_Implicit_Off()
        {
            var expected = "{\"query\":\"query($variableName:argumentType){doSomeAction(argumentName:$variableName)}\",\"variables\":{\"variableName\":{\"a\":\"a\",\"b\":\"b\"}}}";
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
                        Data = (string)null
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());

            // Act
            var query = client.CreateQuery(builder =>
                builder.Field("doSomeAction", field =>
                {
                    field.Argument("argumentName", "argumentType", "variableName", isRequired: true, inlineArgument: false);
                }), "randomurl", arguments: new GraphQLQueryArgument("variableName", new { a = "a", b = "b" }));
            var result = await query.Execute();

            // Assert
            Assert.Equal(null, result);
        }

        [Fact]
        public async Task Test_GraphQL_Builder_Returns_Exception_When_Error_Occurs()
        {
            // Arrange
            var expected =
                "{\"query\":\"query{field}\"}";
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
                            Field = "FieldValue"
                        },
                        Errors = new[]
                        {
                            new
                            {
                                message = "This is not a valid query!",
                                locations = new []
                                {
                                    new
                                    {
                                        line = 1,
                                        column = 0
                                    },
                                    new
                                    {
                                        line = 1,
                                        column = 1
                                    }
                                }
                            }
                        }
                    })
                });

            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());

            // Act
            var query = client.CreateQuery(builder =>
                builder.Field("field"), "randomurl");

            // Assert
            await Assert.ThrowsAsync<GraphQLErrorException>(() => query.Execute());
        }
    }
}
