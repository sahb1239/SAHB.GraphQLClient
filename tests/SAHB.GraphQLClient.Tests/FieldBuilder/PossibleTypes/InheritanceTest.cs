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
            Assert.Equal(typeof(PossibleOtherQuery1), firstPossibleType.Type);
            Assert.Equal(typeof(PossibleOtherQuery1).Name, firstPossibleType.TypeName);
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
            Assert.Equal(typeof(PossibleOtherQuery2), firstPossibleType.Type);
            Assert.Equal(typeof(PossibleOtherQuery2).Name, firstPossibleType.TypeName);
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
    }
}
