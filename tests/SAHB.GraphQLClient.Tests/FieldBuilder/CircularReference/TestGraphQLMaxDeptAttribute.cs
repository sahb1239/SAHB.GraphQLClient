using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.FieldBuilder.CircularReference
{
    public class TestGraphQLMaxDeptAttribute
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public TestGraphQLMaxDeptAttribute()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Should_Not_Throw_When_Max_Depth_Has_Been_Set()
        {
            // Arrange / Act / Assert
            _fieldBuilder.GenerateSelectionSet(typeof(Hello));
        }

        [Fact]
        public void Test_Correct_Number_Of_SelectionSets()
        {
            // Arrange / Act
            var fields = _fieldBuilder.GenerateSelectionSet(typeof(Hello));

            // Assert
            Assert.Single(fields);

            // Depth 2
            var field = fields.First();
            Assert.Single(field.SelectionSet);

            // Depth 3
            field = field.SelectionSet.First();
            Assert.Empty(field.SelectionSet);
        }


        public class Hello
        {
            [GraphQLMaxDepth(3)]
            public Hello SayHello { get; set; }
        }
    }
}
