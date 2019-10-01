using FakeItEasy;
using SAHB.GraphQLClient.Extentions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
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
            var fieldBuilderMock = A.Fake<IGraphQLFieldBuilder>(x => x.Strict());
            A.CallTo(() => fieldBuilderMock.GenerateSelectionSet(typeof(string)))
                .Returns(new GraphQLField[]
                {
                    new GraphQLField("Field", "field", null, null),
                });

            var expected = "{\"query\":\"query{field}\"}";

            var actual = _queryGenerator.GetQuery<string>(fieldBuilderMock);

            Assert.Equal(expected, actual);
        }
    }
}
