using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.NonNested
{
    public class NonNestedFieldBuilder2StringsTest
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public NonNestedFieldBuilder2StringsTest()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Check_Count_Is_2()
        {
            // Get all fields for the type GraphQLStringQuery
            var fields = _fieldBuilder.GetFields(typeof(GraphQLStringQuery)).ToList();

            // Check if contains exactly 2 fields
            Assert.Equal(2, fields.Count);
        }

        [Fact]
        public void Check_Field_Is_camelCase()
        {
            // Get all fields for the type GraphQLStringQuery
            var fields = _fieldBuilder.GetFields(typeof(GraphQLStringQuery)).ToList();

            // Check if containing the 2 fields - it's automatically changing to camelCase
            Assert.Contains(fields, field => field.Field == nameof(GraphQLStringQuery.Str1).ToLower());
            Assert.Contains(fields, field => field.Field == nameof(GraphQLStringQuery.Str2).ToLower());
        }

        [Fact]
        public void Check_Alias_Is_PropertyName()
        {
            // Get all fields for the type GraphQLStringQuery
            var fields = _fieldBuilder.GetFields(typeof(GraphQLStringQuery)).ToList();

            // Check if containing the 2 fields - it's automatically changing to camelCase
            Assert.Contains(fields, field => field.Alias == nameof(GraphQLStringQuery.Str1));
            Assert.Contains(fields, field => field.Alias == nameof(GraphQLStringQuery.Str2));
        }

        [Fact]
        public void Check_SubFields_Is_Empty()
        {
            // Get all fields for the type GraphQLStringQuery
            var fields = _fieldBuilder.GetFields(typeof(GraphQLStringQuery)).ToList();

            // Check if fields is empty on both fields since it's strings (which internally is IEnumerable)
            Assert.Empty(fields.SelectMany(e => e.Fields));
        }

        [Fact]
        public void Check_Arguments_Is_Empty()
        {
            // Get all fields for the type GraphQLStringQuery
            var fields = _fieldBuilder.GetFields(typeof(GraphQLStringQuery)).ToList();

            // Arguments should also be empty
            Assert.Empty(fields.SelectMany(e => e.Arguments));
        }

        public class GraphQLStringQuery
        {
            public string Str1 { get; set; }
            public string Str2 { get; set; }
        }
    }
}
