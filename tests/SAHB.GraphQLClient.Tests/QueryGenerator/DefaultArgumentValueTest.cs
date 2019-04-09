using System;
using System.Collections.Generic;
using System.Text;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator
{
    public class DefaultArgumentValueTest
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;

        public DefaultArgumentValueTest()
        {
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        [Fact]
        public void Test_QueryGenerator_Argument_Default_Value()
        {
            var field = new GraphQLField(alias: null, field: "field1", fields: null, arguments: new List<GraphQLFieldArguments>() {new GraphQLFieldArguments("argumentName", "argumentType", "variableName", true, true, "DefaultValue")});

            var expected = "{\"query\":\"query{field1(argumentName:\\\"DefaultValue\\\")}\"}";

            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, new[] { field });

            Assert.Equal(expected, actual);
        }
    }
}
