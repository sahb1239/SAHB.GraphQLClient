using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.QueryGenerator
{
    public class FilterTests
    {
        private readonly IGraphQLQueryGeneratorFromFields _queryGenerator;

        public FilterTests()
        {
            _queryGenerator = new GraphQLQueryGeneratorFromFields();
        }

        [Fact]
        public void Filter_Root()
        {
            // Arrange
            var includedField = new GraphQLField(field: "includedField", alias: null, fields: null, arguments: null);
            var notIncludedField = new GraphQLField(field: "notIncludedField", alias: null, fields: null, arguments: null);

            var expected = "{\"query\":\"query{includedField}\"}";

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, new[] { includedField, notIncludedField }, filter: field => field.Field == "includedField");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Filter_Scalars_SelectionSet_Included()
        {
            // Arrange
            var includedField = new GraphQLField(field: "includedField", alias: null, arguments: null,
                fields: new[] {
                    new GraphQLField(field: "subfield", alias: null, fields: null, arguments: null)
                });

            var expected = "{\"query\":\"query{includedField{subfield}}\"}";

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, new[] { includedField }, filter: field => field.Field == "includedField");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Scalars_Included_Fields_With_SelectionSet_Excluded()
        {
            // Arrange
            var includedField = new GraphQLField(field: "includedField", alias: null, arguments: null,
                fields: new[] {
                    new GraphQLField(field: "subfield", alias: null, fields: null, arguments: null),
                    new GraphQLField(field: "subfieldWithSelectionSet", alias: null, arguments: null, fields: new [] {
                        new GraphQLField(field: "subfield", alias: null, fields: null, arguments: null),
                    })
                });

            var expected = "{\"query\":\"query{includedField{subfield}}\"}";

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, new[] { includedField }, filter: field => field.Field == "includedField");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Objects_Included_If_No_Scalars_Is_Found()
        {
            // Arrange
            var includedField = new GraphQLField(field: "includedField", alias: null, arguments: null,
                fields: new[] {
                    new GraphQLField(field: "subfieldWithSelectionSet", alias: null, arguments: null, fields: new [] {
                        new GraphQLField(field: "subfield", alias: null, fields: null, arguments: null),
                    })
                });

            var expected = "{\"query\":\"query{includedField{subfieldWithSelectionSet{subfield}}}\"}";

            // Act
            var actual = _queryGenerator.GenerateQuery(GraphQLOperationType.Query, new[] { includedField }, filter: field => field.Field == "includedField");

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
