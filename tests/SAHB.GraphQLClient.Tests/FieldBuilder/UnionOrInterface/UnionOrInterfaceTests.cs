using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAHB.GraphQL.Client.Exceptions;
using SAHB.GraphQL.Client.FieldBuilder.Attributes;
using SAHB.GraphQLClient.FieldBuilder;
using Xunit;

namespace SAHB.GraphQL.Client.Tests.FieldBuilder.UnionOrInterface
{
    public class UnionOrInterfaceTests
    {
        public class Query1
        {
            [GraphQLUnionOrInterface("Obj", typeof(object))]
            public string Hello { get; set; }
        }

        [Fact]
        public void Property_With_UnionOrInterface_On_Property_Should_Be_Included_In_Fields()
        {
            // Arrange
            var fieldBuilder = new GraphQLFieldBuilder();

            // Act
            var fields = fieldBuilder.GenerateSelectionSet(typeof(Query1));

            // Assert
            Assert.Equal(1, fields.Count());

            // Test target types
            Assert.Equal(1, fields.First().TargetTypes.Count);
            Assert.Equal("Obj", fields.First().TargetTypes.First().Key);
            Assert.Equal(typeof(Object), fields.First().TargetTypes.First().Value.Type);

            // Test default type
            Assert.Equal(typeof(String), fields.First().BaseType);
        }

        public class Query2
        {
            [GraphQLUnionOrInterface("Obj1", typeof(object))]
            [GraphQLUnionOrInterface("Obj2", typeof(IEnumerable<string>))]
            public string Hello { get; set; }
        }

        [Fact]
        public void Property_With_2_UnionOrInterface_On_Property_Should_Be_Included_In_Fields()
        {
            // Arrange
            var fieldBuilder = new GraphQLFieldBuilder();

            // Act
            var fields = fieldBuilder.GenerateSelectionSet(typeof(Query2));

            // Assert
            Assert.Equal(1, fields.Count());

            // Test target types
            Assert.Equal(2, fields.First().TargetTypes.Count);

            Assert.Equal("Obj1", fields.First().TargetTypes.First(e => e.Key == "Obj1").Key);
            Assert.Equal(typeof(Object), fields.First().TargetTypes.First(e => e.Key == "Obj1").Value.Type);

            Assert.Equal("Obj2", fields.First().TargetTypes.First(e => e.Key == "Obj2").Key);
            Assert.Equal(typeof(IEnumerable<string>), fields.First().TargetTypes.First(e => e.Key == "Obj2").Value.Type);

            // Test default type
            Assert.Equal(typeof(String), fields.First().BaseType);
        }

        public class Query3
        {
            [GraphQLUnionOrInterface("Obj4", typeof(IEnumerable<string>))]
            public SubQuery3_1 Hello { get; set; }
        }

        [GraphQLUnionOrInterface("Obj3", typeof(object))]
        public class SubQuery3_1
        {
            public string HelloHello { get; set; }
        }

        [Fact]
        public void Property_With_2_UnionOrInterface_On_Property_And_Class_Should_Be_Included_In_Fields()
        {
            // Arrange
            var fieldBuilder = new GraphQLFieldBuilder();

            // Act
            var fields = fieldBuilder.GenerateSelectionSet(typeof(Query3));

            // Assert
            Assert.Equal(1, fields.Count());

            // Test target types
            Assert.Equal(2, fields.First().TargetTypes.Count);

            Assert.Equal("Obj3", fields.First().TargetTypes.First(e => e.Key == "Obj3").Key);
            Assert.Equal(typeof(Object), fields.First().TargetTypes.First(e => e.Key == "Obj3").Value.Type);

            Assert.Equal("Obj4", fields.First().TargetTypes.First(e => e.Key == "Obj4").Key);
            Assert.Equal(typeof(IEnumerable<string>), fields.First().TargetTypes.First(e => e.Key == "Obj4").Value.Type);

            // Test default type
            Assert.Equal(typeof(SubQuery3_1), fields.First().BaseType);
        }

        public class Query4
        {
            [GraphQLUnionOrInterface("Obj1", typeof(string))]
            [GraphQLUnionOrInterface("Obj1", typeof(IEnumerable<string>))]
            public string Hello { get; set; }
        }

        [Fact]
        public void Should_Throw_When_Duplicate_GraphQLUnionOrInterfaceAttributes_Defined()
        {
            // Arrange
            var fieldBuilder = new GraphQLFieldBuilder();

            // Act / Assert
            Assert.Throws<GraphQLDuplicateTypeNameException>(() =>
            {
                var fields = fieldBuilder.GenerateSelectionSet(typeof(Query4));
            });
        }
    }
}
