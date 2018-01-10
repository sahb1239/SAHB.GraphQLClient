using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
    }
}
