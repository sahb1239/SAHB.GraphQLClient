using System;
using System.Collections.Generic;
using System.Text;
using SAHB.GraphQLClient.Extentions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Tests.Mocks;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator
{
    public class TestCaseInsensitiveAliasFieldMatch
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        public TestCaseInsensitiveAliasFieldMatch()
        {
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        [Fact]
        public void TestCaseInsensitiveAliasField()
        {
            var fields = new[]
            {
                new GraphQLField("Field", "field", null, null),
            };
            var fieldBuilder = new FieldBuilderMock(fields);
            var expected = "{\"query\":\"query{field}\"}";

            var actual = _queryGenerator.GetQuery<string>(fieldBuilder); // Type parameter is ignored since it just returns the fields

            Assert.Equal(expected, actual);
        }
    }
}
