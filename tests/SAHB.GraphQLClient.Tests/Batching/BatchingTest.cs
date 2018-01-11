using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Tests.GraphQLClient.HttpClientMock;
using Xunit;

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
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
                {
                    Data = new
                    {
                        batch0_Part1Field1 = "Value1",
                        batch0_Part1Field2 = "Value2",
                        batch1_Part2Field3 = "Value3",
                        batch1_Part2Field4 = "Value4"
                    }
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields());

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
                @"{""query"":""query($batch0_argumentVariable:Int $batch1_argumentVariable2:Int){batch0_Part1Field1:part1_field1(argumentName:$batch0_argumentVariable) batch0_Part1Field2:part1Field2 batch1_Part2Field3:part2_field3(argumentName:$batch1_argumentVariable2) batch1_Part2Field4:part2Field4}"",""Variables"":""{\""batch0_argumentVariable\"":\""1\"",\""batch1_argumentVariable2\"":\""2\""}""}";
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
                {
                    Data = new
                    {
                        batch0_Part1Field1 = "Value1",
                        batch0_Part1Field2 = "Value2",
                        batch1_Part2Field3 = "Value3",
                        batch1_Part2Field4 = "Value4"
                    }
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields());

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
                @"{""query"":""query($batch0_argumentVariable:Int $batch1_argumentVariable:Int){batch0_Part1Field1:part1_field1(argumentName:$batch0_argumentVariable) batch0_Part1Field2:part1Field2 batch1_Part2Field3:part2_field3(argumentName:$batch1_argumentVariable) batch1_Part2Field4:part2Field4}"",""Variables"":""{\""batch0_argumentVariable\"":\""1\"",\""batch1_argumentVariable\"":\""2\""}""}";
            var httpClientMock = new GraphQLHttpExecutorMock(
                JsonConvert.SerializeObject(new
                {
                    Data = new
                    {
                        batch0_Part1Field1 = "Value1",
                        batch0_Part1Field2 = "Value2",
                        batch1_Part2Field3 = "Value3",
                        batch1_Part2Field4 = "Value4"
                    }
                }), expected);
            var client = new GraphQLHttpClient(httpClientMock, new GraphQLFieldBuilder(),
                new GraphQLQueryGeneratorFromFields());

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
    }
}