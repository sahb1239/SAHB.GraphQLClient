using System.Collections.Generic;
using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.IEnumerable
{
    public class EnuerableTest
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public EnuerableTest()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Correct_Count_With_IEnumerable_Field()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(QueryToTest)).ToList<GraphQLField>();

            // Expect one element in root
            Assert.Equal(1, fields.Count);
        }

        [Fact]
        public void Correct_Count_Fields()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(QueryToTest)).ToList<GraphQLField>();

            // Expect 
            Assert.Equal(1, fields.First().Fields.Count());
        }

        public class QueryToTest
        {
            public IEnumerable<GraphQLEnumerableType> Field1 { get; set; }
        }

        public class GraphQLEnumerableType
        {
            public string Field2 { get; set; }
        }
    }
}
