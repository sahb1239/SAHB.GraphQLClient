using System.Collections.Generic;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryBuilder;
using Xunit;

namespace SAHB.GraphQLClient.Tests.QueryBuilder.IntegrationTests
{
    public class NestedIntegrationTests
    {
        private readonly IGraphQLQueryBuilder _queryBuilder;

        public NestedIntegrationTests()
        {
            var fieldBuilder = new GraphQLFieldBuilder();
            _queryBuilder = new GraphQLQueryBuilder(fieldBuilder);
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
            var expected = "{\"Query\":\"query{Me:me{Name:name Age:age lastname}}\"}";

            var actual = _queryBuilder.GetQuery<Query1>();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NestedQueryTestWithFieldNamesAndIEnumerable()
        {
            var expected = "{\"Query\":\"query{Me:me{Name:name Age:age lastname} Others:other{Name:name Age:age lastname}}\"}";

            var actual = _queryBuilder.GetQuery<Query2>();

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
            var expected = "{\"Query\":\"query{Others:other{Name:name Age:age lastname} Me:me{Name:name Age:age lastname}}\"}";

            var actual = _queryBuilder.GetQuery<Query3>();

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
            var expected = "{\"Query\":\"query{Me:me{Name:name Age:age lastname}}\"}";

            var actual = _queryBuilder.GetQuery<Query4>();

            Assert.Equal(expected, actual);
        }
    }
}
