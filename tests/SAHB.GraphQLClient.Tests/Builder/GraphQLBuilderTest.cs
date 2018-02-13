using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Tests.GraphQLClient.HttpClientMock;
using Xunit;

namespace SAHB.GraphQLClient.Tests.Builder
{
    public class GraphQLBuilderTest
    {
        [Fact]
        public async Task Test_GraphQL_Builder_Get_Result()
        {
            var expected = "{\"query\":\"query{alias1:field1 alias2:field2}\"}";
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
                {
                    Data = new
                    {
                        alias1 = "Value1",
                        alias2 = "Value2"
                    }
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields());

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
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
                {
                    Data = (string)null
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields());

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
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
                {
                    Data = (string)null
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields());

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
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
                {
                    Data = (string)null
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields());

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
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
                {
                    Data = (string)null
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields());

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
        public void Test_GraphQL_Builder_Argument_Required_Throws()
        {
            var expected = "{\"query\":\"query{doSomeAction}\"}";
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
                {
                    Data = (string)null
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields());

            // Act / Assert
            Assert.Throws<GraphQLArgumentsRequiredException>(() => client.CreateQuery(builder =>
                    builder.Field("doSomeAction",
                        field => { field.Argument("argumentName", "argumentType", "variableName", isRequired: true); }),
                "randomurl"));
        }

        [Fact]
        public async Task Test_GraphQL_Builder_Argument_Inlined_Explicit_Off()
        {
            var expected = "{\"query\":\"query($variableName:argumentType){doSomeAction(argumentName:$variableName)}\",\"variables\":{\"variableName\":1}}";
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
                {
                    Data = (string)null
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields());

            // Act
            var query = client.CreateQuery(builder =>
                builder.Field("doSomeAction", field =>
                {
                    field.Argument("argumentName", "argumentType", "variableName", isRequired:true, inlineArgument:false);
                }), "randomurl", arguments: new GraphQLQueryArgument("variableName", 1));
            var result = await query.Execute();

            // Assert
            Assert.Equal(null, result);
        }

        [Fact]
        public async Task Test_GraphQL_Builder_Argument_Inlined_Implicit_Off()
        {
            var expected = "{\"query\":\"query($variableName:argumentType){doSomeAction(argumentName:$variableName)}\",\"variables\":{\"variableName\":{\"a\":\"a\",\"b\":\"b\"}}}";
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
                {
                    Data = (string)null
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(), new GraphQLQueryGeneratorFromFields());

            // Act
            var query = client.CreateQuery(builder =>
                builder.Field("doSomeAction", field =>
                {
                    field.Argument("argumentName", "argumentType", "variableName", isRequired: true, inlineArgument: false);
                }), "randomurl", arguments: new GraphQLQueryArgument("variableName", new {a = "a", b = "b"}));
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
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
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
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields());

            // Act
            var query = client.CreateQuery(builder =>
                builder.Field("field"), "randomurl");

            // Assert
            await Assert.ThrowsAsync<GraphQLErrorException>(() => query.Execute());
        }
    }
}
