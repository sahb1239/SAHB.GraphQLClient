using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.NonNested
{
    public class NonNestedFieldBuilder2IntTest
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public NonNestedFieldBuilder2IntTest()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Check_Count_Is_2()
        {
            // Get all fields for the type GraphQLStringQuery
            var fields = _fieldBuilder.GenerateOperation(typeof(GraphQLIntQuery), GraphQLOperationType.Query).SelectionSet.ToList<GraphQLField>();

            // Check if contains excactly 2 fields
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void Check_Field_Is_camelCase()
        {
            // Get all fields for the type GraphQLStringQuery
            var fields = _fieldBuilder.GenerateOperation(typeof(GraphQLIntQuery), GraphQLOperationType.Query).SelectionSet.ToList<GraphQLField>();

            // Check if containing the 2 fields - it's automatically changing to camelCase
            Assert.Contains(fields, field => field.Field == nameof(GraphQLIntQuery.Int1).ToLower());
            Assert.Contains(fields, field => field.Field == nameof(GraphQLIntQuery.Int2).ToLower());
        }

        [Fact]
        public void Check_Alias_Is_PropertyName()
        {
            // Get all fields for the type GraphQLStringQuery
            var fields = _fieldBuilder.GenerateOperation(typeof(GraphQLIntQuery), GraphQLOperationType.Query).SelectionSet.ToList<GraphQLField>();

            // Check if containing the 2 fields - it's automatically changing to camelCase
            Assert.Contains(fields, field => field.Alias == nameof(GraphQLIntQuery.Int1));
            Assert.Contains(fields, field => field.Alias == nameof(GraphQLIntQuery.Int2));
        }

        [Fact]
        public void Check_SubFields_Is_Empty()
        {
            // Get all fields for the type GraphQLStringQuery
            var fields = _fieldBuilder.GenerateOperation(typeof(GraphQLIntQuery), GraphQLOperationType.Query).SelectionSet.ToList<GraphQLField>();

            // Check if fields is empty on both fields since it's strings (which internally is IEnumerable)
            Assert.Empty(fields.SelectMany(e => e.SelectionSet));
        }

        [Fact]
        public void Check_Arguments_Is_Empty()
        {
            // Get all fields for the type GraphQLStringQuery
            var fields = _fieldBuilder.GenerateOperation(typeof(GraphQLIntQuery), GraphQLOperationType.Query).SelectionSet.ToList<GraphQLField>();

            // Arguments should also be empty
            Assert.Empty(fields.SelectMany(e => e.Arguments));
        }

        public class GraphQLIntQuery
        {
            public int Int1 { get; set; }
            public int Int2 { get; set; }
        }
    }
}