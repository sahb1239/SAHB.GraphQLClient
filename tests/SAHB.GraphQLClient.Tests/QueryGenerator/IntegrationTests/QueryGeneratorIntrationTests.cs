using System;
using System.Collections.Generic;
using System.Text;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Extentions;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator.IntegrationTests
{
    public class QueryGeneratorIntrationTests
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public QueryGeneratorIntrationTests()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        public class QueryWithArgumentInlineFalse
        {
            [GraphQLArguments("argumentName1", "argumentType1", "variableName1", true, false)]
            public string Field { get; set; }
        }

        public class QueryWithArgumentInlineTrue
        {
            [GraphQLArguments("argumentName2", "argumentType2", "variableName2", true, true)]
            public string Field { get; set; }
        }

        [Fact]
        public void Test_Query_Inline_Argument_False()
        {
            var expected =
                "{\"query\":\"query($variableName1:argumentType1){field(argumentName1:$variableName1)}\",\"variables\":{\"variableName1\":\"Test1\"}}";

            var actual = _queryGenerator.GetQuery<QueryWithArgumentInlineFalse>(_fieldBuilder, new GraphQLQueryArgument("variableName1", "Test1"));

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Query_Inline_Argument_True()
        {
            var expected =
                "{\"query\":\"query{field(argumentName2:\\\"Test2\\\")}\"}";

            var actual = _queryGenerator.GetQuery<QueryWithArgumentInlineTrue>(_fieldBuilder, new GraphQLQueryArgument("variableName2", "Test2"));

            Assert.Equal(expected, actual);
        }
    }
}
