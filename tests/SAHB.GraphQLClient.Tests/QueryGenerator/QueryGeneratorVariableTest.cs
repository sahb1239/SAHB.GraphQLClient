using System.Collections.Generic;
using Newtonsoft.Json;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator
{
    public class QueryGeneratorVariableTest
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        public QueryGeneratorVariableTest()
        {
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        [Fact]
        public void Test_Query_Generator_String_Variable()
        {
            // Arrange
            var expected = "{\"query\":\"query($variableName:argumentType){alias:field(argumentName:$variableName)}\",\"variables\":{\"variableName\":\"value\"}}";
            var fields = new []
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName")
                    })
            };

            // Act
            var actual = _queryGenerator.GetQuery(fields, new GraphQLQueryArgument("variableName", "value"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Query_Generator_Two_String_Variable()
        {
            // Arrange
            var expected =
                "{\"query\":\"query($variableName1:argumentType1 $variableName2:argumentType2){alias:field(argumentName1:$variableName1 argumentName2:$variableName2)}\",\"variables\":{\"variableName1\":\"value1\",\"variableName2\":\"value2\"}}";
            var fields = new []
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName1", "argumentType1", "variableName1"),
                        new GraphQLFieldArguments("argumentName2", "argumentType2", "variableName2")
                    })
            };

            // Act
            var actual = _queryGenerator.GetQuery(fields, new GraphQLQueryArgument("variableName1", "value1"),
                new GraphQLQueryArgument("variableName2", "value2"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Query_Generator_String_Converted_Variable()
        {
            // Arrange
            var expected = "{\"query\":\"query($variableName:argumentType){alias:field(argumentName:$variableName)}\",\"variables\":{\"variableName\":\"{\\\"arg1\\\":\\\"value1\\\",\\\"arg2\\\":\\\"value2\\\"}\"}}";
            var fields = new []
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName")
                    })
            };

            // Act
            var actual = _queryGenerator.GetQuery(fields, new GraphQLQueryArgument("variableName", JsonConvert.SerializeObject(new {arg1 = "value1", arg2 = "value2"})));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Variable_Complex_Dynamic_Type()
        {
            // Arrange
            var expected = "{\"query\":\"query($variableName:argumentType){field(argumentName:$variableName)}\",\"variables\":{\"variableName\":{\"field1\":\"value1\",\"field2\":\"value2\"}}}";
            var fields = new []
            {
                new GraphQLField(alias: null, field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName")
                    })
            };

            // Act
            var actual = _queryGenerator.GetQuery(fields,
                new GraphQLQueryArgument("variableName", new { field1 = "value1", field2 = "value2" }));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Variable_Complex_Static_Type()
        {
            // Arrange
            var expected = "{\"query\":\"query($variableName:argumentType){field(argumentName:$variableName)}\",\"variables\":{\"variableName\":{\"Field1\":\"value1\",\"Field2\":\"value2\"}}}";
            var fields = new []
            {
                new GraphQLField(alias: null, field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName")
                    })
            };

            // Act
            var actual = _queryGenerator.GetQuery(fields,
                new GraphQLQueryArgument("variableName", new StaticArgument { Field1 = "value1", Field2 = "value2" }));

            // Assert
            Assert.Equal(expected, actual);
        }

        public class StaticArgument
        {
            public string Field1 { get; set; }
            public string Field2 { get; set; }
        }
    }
}
