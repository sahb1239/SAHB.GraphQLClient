using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAHB.GraphQLClient.FieldBuilder;
using Xunit;

namespace SAHB.GraphQLClient.Tests.FieldBuilder.Inheritance
{
    public class InheritanceTest
    {
        private readonly IGraphQLFieldBuilder _fieldBuilder;

        public InheritanceTest()
        {
            _fieldBuilder = new GraphQLFieldBuilder();
        }

        [Fact]
        public void Correct_Count_Fields()
        {
            // Get all fields for the type QueryToTest
            var fields = _fieldBuilder.GetFields(typeof(QueryToTest)).ToList<GraphQLField>();

            // Expect 
            Assert.Equal(2, fields.Count());
        }

        public class InheritedFromQuery
        {
            public string Field1 { get; set; }
        }

        public class QueryToTest : InheritedFromQuery
        {
            public string Field2 { get; set; }
        }
    }
}
