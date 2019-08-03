using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.Nested
{
    public class Nested1Test
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public Nested1Test()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Correct_Count_With_Nested_Field()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest)).ToList();

            // Expect one element in root
            Assert.Equal(1, fields.Count);
        }

        [Fact]
        public void Correct_Count_Fields()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(QueryToTest)).ToList();

            // Expect 
            Assert.Equal(1, fields.First().SelectionSet.Count());
        }

        public class QueryToTest
        {
            public GraphQLNestedType Field1 { get; set; }
        }

        public class GraphQLNestedType
        {
            public string Field2 { get; set; }
        }
    }
}
