using System.Collections.Generic;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Exceptions;
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
            var expected = "{\"query\":\"query{alias:field(argumentName:\\\"value\\\")}\"}";
            var fields = new []
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName")
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", "value"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Query_Generator_Two_String_Variable()
        {
            // Arrange
            var expected =
                "{\"query\":\"query{alias:field(argumentName1:\\\"value1\\\" argumentName2:\\\"value2\\\")}\"}";
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
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName1", "value1"),
                new GraphQLQueryArgument("variableName2", "value2"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Query_Generator_String_Converted_Variable()
        {
            // Arrange
            var expected = "{\"query\":\"query{alias:field(argumentName:\\\"{\\\\\\\"arg1\\\\\\\":\\\\\\\"value1\\\\\\\",\\\\\\\"arg2\\\\\\\":\\\\\\\"value2\\\\\\\"}\\\")}\"}";
            var fields = new []
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName")
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", JsonConvert.SerializeObject(new {arg1 = "value1", arg2 = "value2"})));

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
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields,
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
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields,
                new GraphQLQueryArgument("variableName", new StaticArgument { Field1 = "value1", Field2 = "value2" }));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Variable_Not_Set_Should_Throw()
        {
            // Arrange
            var fields = new[]
            {
                new GraphQLField(alias: null, field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", true)
                    })
            };

            // Act / assert
            Assert.Throws<GraphQLArgumentsRequiredException>(() =>
            {
                var query = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields);
            });
        }

        [Fact]
        public void Test_QueryGenerator_Should_Throw_When_No_Argument_Is_Supplied_To_Required_Argument()
        {
            // Arrange
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", true)
                    })
            };

            // Act / Assert
            Assert.Throws<GraphQLArgumentsRequiredException>(() => _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields));
        }

        [Fact]
        public void Test_QueryGenerator_Should_Not_Throw_When_Argument_Is_Supplied_To_Required_Argument()
        {
            // Arrange
            var expected = "{\"query\":\"query{alias:field(argumentName:\\\"value\\\")}\"}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", true)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", "value"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Argument_Should_Not_Be_Set_When_Not_Required_And_Not_Set()
        {
            // Arrange
            var expected = "{\"query\":\"query{alias:field}\"}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", false)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Inline_Int_Argument_Explicit()
        {
            // Arrange
            var expected = "{\"query\":\"query{alias:field(argumentName:1)}\"}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:true)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", 1));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Inline_String_Argument_Explicit()
        {
            // Arrange
            var expected = "{\"query\":\"query{alias:field(argumentName:\\\"test\\\")}\"}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:true)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", "test"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Inline_Int_Argument_Implicit()
        {
            // Arrange
            var expected = "{\"query\":\"query{alias:field(argumentName:1)}\"}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:null)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", 1));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Inline_String_Argument_Implicit()
        {
            // Arrange
            var expected = "{\"query\":\"query{alias:field(argumentName:\\\"test\\\")}\"}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:null)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", "test"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Inline_DynamicType_Argument_Implicit_Not_Inlined()
        {
            // Arrange
            var expected = "{\"query\":\"query($variableName:argumentType){alias:field(argumentName:$variableName)}\",\"variables\":{\"variableName\":{\"arg1\":\"val1\",\"arg2\":2}}}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:null)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", new {arg1 = "val1", arg2 = 2}));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Inline_String_Argument_Explicit_Not_Inlined()
        {
            // Arrange
            var expected = "{\"query\":\"query($variableName:argumentType){alias:field(argumentName:$variableName)}\",\"variables\":{\"variableName\":\"test\"}}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:false)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", "test"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_One_Explicit_Not_Inlined_And_One_Implicit_Inlined()
        {
            // Arrange
            var expected = "{\"query\":\"query($variableName:argumentType){alias:field(argumentName:$variableName argumentName1:\\\"test1\\\")}\",\"variables\":{\"variableName\":\"test\"}}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:false),
                        new GraphQLFieldArguments("argumentName1", "argumentType1", "variableName1", isRequired:false)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", "test"), new GraphQLQueryArgument("variableName1", "test1"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_One_Explicit_Not_Inlined_And_One_Implicit_Inlined_Same_Variable_Name()
        {
            // Arrange
            var expected = "{\"query\":\"query($variableName:argumentType){alias:field(argumentName:$variableName argumentName1:\\\"test\\\")}\",\"variables\":{\"variableName\":\"test\"}}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:false),
                        new GraphQLFieldArguments("argumentName1", "argumentType1", "variableName", isRequired:false)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", "test"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Should_Throw_When_Multiple_Same_Variables_Has_Been_Defined()
        {
            // Arrange
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false)
                    })
            };

            // Act / Assert
            Assert.Throws<GraphQLDuplicateVariablesException>(() => _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields,
                new GraphQLQueryArgument("variableName", "test"), new GraphQLQueryArgument("variableName", "test")));
        }

        [Fact]
        public void Test_Should_Throw_When_Multiple_Same_Variables_Has_Been_Defined_Only_One_Variable_In_Exception()
        {
            // Arrange
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false)
                    })
            };

            // Act / Assert
            var exception = Assert.Throws<GraphQLDuplicateVariablesException>(() => _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields,
                new GraphQLQueryArgument("variableName", "test"), new GraphQLQueryArgument("variableName", "test")));

            // Should only contain one duplicate variable name
            Assert.Equal(1, exception.DuplicateVariableNames.Count);
        }

        [Fact]
        public void Test_Should_Throw_When_Multiple_Same_Variables_Correct_Duplicate_Variable_Name_In_Exception()
        {
            // Arrange
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false)
                    })
            };

            // Act / Assert
            var exception = Assert.Throws<GraphQLDuplicateVariablesException>(() => _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields,
                new GraphQLQueryArgument("variableName", "test"), new GraphQLQueryArgument("variableName", "test")));

            // Should only contain one duplicate variable name
            Assert.Contains("variableName", exception.DuplicateVariableNames);
        }

        public class StaticArgument
        {
            public string Field1 { get; set; }
            public string Field2 { get; set; }
        }
    }
}
