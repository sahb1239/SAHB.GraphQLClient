using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using System.Collections.Generic;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator
{
    public class FieldTests
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        public FieldTests()
        {
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        [Fact]
        public void Does_Not_Finds_Argument_With_Specific_Field()
        {
            // Arrange
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:true)
                    })
            };

            // Act / Assert
            Assert.Throws<GraphQLArgumentVariableNotFoundException>(() =>
                _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", "otherAliasWhichShouldNotBeFound", "test")));
        }

        [Fact]
        public void Check_Finds_Argument_With_Specific_Field()
        {
            // Arrange
            var expected = @"{""query"":""query{alias:field(argumentName:\""test\"")}""}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:true)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields, new GraphQLQueryArgument("variableName", "alias", "test"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Check_Finds_Argument_With_Same_Variable_Name_On_Multiple_Fields()
        {
            // Arrange
            var expected = @"{""query"":""query{alias:field(argumentName:\""test\"") alias2:field(argumentName:\""test2\"")}""}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:true)
                    }),
                new GraphQLField(alias: "alias2", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType2", "variableName", isRequired:false, inlineArgument:true)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields,
                new GraphQLQueryArgument("variableName", "alias", "test"),
                new GraphQLQueryArgument("variableName", "alias2", "test2"));

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Ignores_Argument_From_Other_Field_If_Not_Set()
        {
            // Arrange
            var expected = @"{""query"":""query{alias:field(argumentName:\""test\"") alias2:field}""}";
            var fields = new[]
            {
                new GraphQLField(alias: "alias", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType", "variableName", isRequired:false, inlineArgument:true)
                    }),
                new GraphQLField(alias: "alias2", field: "field", fields: null,
                    arguments: new List<GraphQLFieldArguments>
                    {
                        new GraphQLFieldArguments("argumentName", "argumentType2", "variableName", isRequired:false, inlineArgument:true)
                    })
            };

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, fields,
                new GraphQLQueryArgument("variableName", "alias", "test"));

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
