using System.Collections.Generic;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Extentions;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryGenerator.IntegrationTests
{
    public class NestedIntegrationTests
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public NestedIntegrationTests()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        public class Query1
        {
            public Person Me { get; set; }
        }

        public class Person
        {
            public string Name { get; set; }
            public uint Age { get; set; }
            public string lastname { get; set; }
        }

        public class Query2
        {
            public Person Me { get; set; }
            
            [GraphQLFieldName("other")]
            public IEnumerable<Person> Others { get; set; }
        }

        [Fact]
        public void NestedQueryTestWithFieldNames()
        {
            var expected = "{\"query\":\"query{me{name age lastname}}\"}";

            var actual = _queryGenerator.GetQuery<Query1>(_fieldBuilder);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NestedQueryTestWithFieldNamesAndIEnumerable()
        {
            var expected = "{\"query\":\"query{me{name age lastname} Others:other{name age lastname}}\"}";

            var actual = _queryGenerator.GetQuery<Query2>(_fieldBuilder);

            Assert.Equal(expected, actual);
        }

        public class Query3 : Query1
        {
            [GraphQLFieldName("other")]
            public IEnumerable<Person> Others { get; set; }
        }

        [Fact]
        public void NestedQueryTestWithFieldNamesAndIEnumerableAndInherited()
        {
            var expected = "{\"query\":\"query{Others:other{name age lastname} me{name age lastname}}\"}";

            var actual = _queryGenerator.GetQuery<Query3>(_fieldBuilder);

            Assert.Equal(expected, actual);
        }

        public class Query4
        {
            public Person Me { get; set; }

            [GraphQLFieldIgnore]
            public string Ignored { get; set; }
        }

        [Fact]
        public void NestedQueryTestWithFieldNamesWithIgnored()
        {
            var expected = "{\"query\":\"query{me{name age lastname}}\"}";

            var actual = _queryGenerator.GetQuery<Query4>(_fieldBuilder);

            Assert.Equal(expected, actual);
        }
    }
}
