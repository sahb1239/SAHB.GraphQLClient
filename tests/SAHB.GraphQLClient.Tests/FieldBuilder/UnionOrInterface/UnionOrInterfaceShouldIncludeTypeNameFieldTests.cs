using SAHB.GraphQL.Client.FieldBuilder.Attributes;
using SAHB.GraphQLClient.FieldBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.FieldBuilder.UnionOrInterface
{
    public class UnionOrInterfaceShouldIncludeTypeNameFieldTests
    {
        public class Query1
        {
            [GraphQLUnionOrInterface("subclass1", typeof(SubClass))]
            [GraphQLUnionOrInterface("subclass2", typeof(SubSubClass))]
            public SubClass Hello { get; set; }
        }

        public class SubClass
        {
            public string Test2 { get; set; }
        }

        public class SubSubClass : SubClass
        {
            public string Test2 { get; set; }
        }

        [Fact]
        public void Property_With_UnionOrInterface_On_Property_Should_Be_Included_In_Fields()
        {
            // Arrange
            var fieldBuilder = new GraphQLFieldBuilder();

            // Act
            var fields = fieldBuilder.GetFields(typeof(Query1));

            // Assert
            Assert.Equal(1, fields.Count());

            // Test field __typename is added
            Assert.True(fields.First().SelectionSet.Any(field => field.Field == "__typename"));

            // Test target types
            Assert.Equal(2, fields.First().TargetTypes.Count);
            Assert.Equal("subclass1", fields.First().TargetTypes.First().Key);
            Assert.Equal("subclass2", fields.First().TargetTypes.Last().Key);

            // Test default type
            Assert.Equal(typeof(SubClass), fields.First().DefaultTargetType);
        }
    }
}
