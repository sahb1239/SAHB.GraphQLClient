using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.PossibleTypes
{
    public class PossibleTypesTests
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public PossibleTypesTests()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Correct_Possible_Types_Property()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(QueryToTest1)).ToList<GraphQLField>();

            // Expect 
            Assert.Equal(1, fields.Count());

            // Get first field
            var firstField = fields.First();
            Assert.Equal(1, firstField.PossibleTypes.Count);

            // Get first possible type
            var firstPossibleType = firstField.PossibleTypes.First();
            
            // Assert the number of fields (should be 2 since we inherit)
            Assert.Equal(2, firstPossibleType.Fields.Count);

            // Check if Field2 and Field3 is in the possibleType
            Assert.True(firstPossibleType.Fields.Any(field => field.Alias == nameof(PossibleOtherQuery1.Field2)));
            Assert.True(firstPossibleType.Fields.Any(field => field.Alias == nameof(PossibleOtherQuery1.Field3)));
        }

        public class QueryToTest1
        {
            [GraphQLPossibleTypes(typeof(PossibleOtherQuery1))]
            public NestedField1 Field1 { get; set; }
        }

        public class NestedField1
        {
            public string Field2 { get; set; }
        }

        public class PossibleOtherQuery1 : NestedField1
        {
            public string Field3 { get; set; }
        }

        [Fact]
        public void Correct_Possible_Types_Class()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(QueryToTest2)).ToList<GraphQLField>();

            // Expect 
            Assert.Equal(1, fields.Count());

            // Get first field
            var firstField = fields.First();
            Assert.Equal(1, firstField.PossibleTypes.Count);

            // Get first possible type
            var firstPossibleType = firstField.PossibleTypes.First();

            // Assert the number of fields (should be 2 since we inherit)
            Assert.Equal(2, firstPossibleType.Fields.Count);

            // Check if Field2 and Field3 is in the possibleType
            Assert.True(firstPossibleType.Fields.Any(field => field.Alias == nameof(PossibleOtherQuery2.Field2)));
            Assert.True(firstPossibleType.Fields.Any(field => field.Alias == nameof(PossibleOtherQuery2.Field3)));
        }

        public class QueryToTest2
        {
            public NestedField2 Field1 { get; set; }
        }

        [GraphQLPossibleTypes(typeof(PossibleOtherQuery2))]
        public class NestedField2
        {
            public string Field2 { get; set; }
        }

        public class PossibleOtherQuery2 : NestedField2
        {
            public string Field3 { get; set; }
        }

        [Fact]
        public void Correct_Possible_Types_Class_CustomName()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(QueryToTest3)).ToList<GraphQLField>();

            // Expect 
            Assert.Equal(1, fields.Count());

            // Get first field
            var firstField = fields.First();
            Assert.Equal(1, firstField.PossibleTypes.Count);

            // Get first possible type
            var firstPossibleType = firstField.PossibleTypes.First();

            // Assert the number of fields (should be 2 since we inherit)
            Assert.Equal(2, firstPossibleType.Fields.Count);

            // Check if Field2 and Field3 is in the possibleType
            Assert.True(firstPossibleType.Fields.Any(field => field.Alias == nameof(PossibleOtherQuery3.Field2)));
            Assert.True(firstPossibleType.Fields.Any(field => field.Alias == nameof(PossibleOtherQuery3.Field3)));

            // Assert the name was overrided
            Assert.Equal("CustomTypeName", firstPossibleType.TypeName);
        }

        public class QueryToTest3
        {
            public NestedField3 Field1 { get; set; }
        }

        [GraphQLPossibleTypes(typeof(PossibleOtherQuery3))]
        public class NestedField3
        {
            public string Field2 { get; set; }
        }

        [GraphQLTypeName("CustomTypeName")]
        public class PossibleOtherQuery3 : NestedField3
        {
            public string Field3 { get; set; }
        }

        [Fact]
        public void Possible_Types_Including_Own_Type()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(QueryToTest4)).ToList<GraphQLField>();

            // Expect 
            Assert.Equal(1, fields.Count());

            // Get first field
            var firstField = fields.First();
            Assert.Equal(2, firstField.PossibleTypes.Count);
        }

        public class QueryToTest4
        {
            public NestedField4 Field1 { get; set; }
        }

        [GraphQLPossibleTypes(typeof(NestedField4), typeof(PossibleOtherQuery4))]
        public class NestedField4
        {
            public string Field2 { get; set; }
        }
        
        public class PossibleOtherQuery4 : NestedField4
        {
            public string Field3 { get; set; }
        }

        [Fact]
        public void Possible_Types_Should_Not_Work_For_Base_Type()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(QueryToTest5)).ToList<GraphQLField>();

            // Expect 
            Assert.Equal(1, fields.Count());

            // We have no way of seeing the GraphQLPossibleTypes which is intentional
        }

        [GraphQLPossibleTypes(typeof(PossibleOtherQuery5))]
        public class QueryToTest5
        {
            public PossibleOtherQuery5 Field1 { get; set; }
        }

        public class PossibleOtherQuery5 : QueryToTest5
        {
            public string Field2 { get; set; }
        }
    }
}
