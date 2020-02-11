using System;
using System.Linq;
using System.Linq.Expressions;
using SAHB.GraphQLClient.Filtering;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using Xunit;

namespace SAHB.GraphQLClient.Tests.Filtering
{
    public class QueryGeneratorFilterTests
    {
        private readonly QueryGeneratorFilter queryGeneratorFilter;
        private readonly GraphQLFieldBuilder fieldBuilder;

        public QueryGeneratorFilterTests()
        {
            queryGeneratorFilter = new QueryGeneratorFilter();
            fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Single_Member_Name()
        {
            // Arrange
            Expression<Func<GraphQLQuery, GraphQLQuery>> expression = e => new GraphQLQuery
            {
                Field2 = e.Field2
            };
            var fields = fieldBuilder.GenerateSelectionSet(typeof(GraphQLQuery));

            // Act
            var filter = queryGeneratorFilter.GetFilter(expression);

            // Assert
            Assert.True(filter(fields.Single(e => e.Alias == nameof(GraphQLQuery.Field2))));
            Assert.False(filter(fields.Single(e => e.Alias == nameof(GraphQLQuery.Field))));
        }

        [Fact]
        public void Nested_Member_Name()
        {
            // Arrange
            Expression<Func<GraphQLQuery, GraphQLQuery>> expression = e => new GraphQLQuery
            {
                Field = new GraphQLQuery
                {
                    Field2 = e.Field.Field2
                }
            };
            var fields = fieldBuilder.GenerateSelectionSet(typeof(GraphQLQuery));

            // Act
            var filter = queryGeneratorFilter.GetFilter(expression);

            // Assert
            Assert.True(filter(fields.Single(e => e.Alias == nameof(GraphQLQuery.Field))));
            Assert.True(
                filter(fields
                    .Single(e => e.Alias == nameof(GraphQLQuery.Field))
                    .SelectionSet
                    .Single(e => e.Alias == nameof(GraphQLQuery.Field2))));
            Assert.False(filter(fields.Single(e => e.Alias == nameof(GraphQLQuery.Field2))));
        }

        class GraphQLQuery
        {
            [GraphQLMaxDepth(10)]
            public GraphQLQuery Field { get; set; }
            public string Field2 { get; set; }
        }
    }
}
