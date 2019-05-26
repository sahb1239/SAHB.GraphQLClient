using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Tests.GraphQLClient.HttpClientMock;
using Xunit;

namespace SAHB.GraphQLClient.Tests.GraphQLClient
{
    public class GraphQLClientTest
    {
        [Fact]
        public async Task Test_Client_Should_Deserilize_Errors()
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
                        },
                        new
                        {
                            message = "And this is a second error message",
                            locations = new []
                            {
                                new
                                {
                                    line = 1,
                                    column = 10
                                }
                            }
                        }
                    }
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());

            // Act
            var query = client.CreateQuery<Query>("url");

            // Assert
            var exception = await Assert.ThrowsAsync<GraphQLErrorException>(() => query.Execute());
            Assert.Equal(expected, exception.Query);
            Assert.Equal(2, exception.Errors.Count());

            // Validate error 1
            Assert.Equal("This is not a valid query!", exception.Errors.First().Message);
            Assert.Equal(true, exception.Errors.First().ContainLocations);
            Assert.Equal(2, exception.Errors.First().Locations.Count());
            Assert.Equal(1, exception.Errors.First().Locations.First().Line);
            Assert.Equal(0, exception.Errors.First().Locations.First().Column);
        }

        [Fact]
        public async Task Test_GraphQLClient_Should_Throw_When_Error_Is_Returned()
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
                    Errors = new []
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
                new GraphQLQueryGeneratorFromFields(), new GraphQLDeserilization());

            // Act
            var query = client.CreateQuery<Query>("url");

            // Assert
            await Assert.ThrowsAsync<GraphQLErrorException>(() => query.Execute());
        }

        public class Query
        {
            public string Field { get; set; }
        }
    }
}
