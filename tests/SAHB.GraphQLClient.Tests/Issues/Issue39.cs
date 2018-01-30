using System;
using System.Collections.Generic;
using System.Text;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using Xunit;

namespace SAHB.GraphQLClient.Tests.Issues
{
    public class Issue39
    {
        [Fact]
        public void Test_Single_Field_Should_Not_Generate_Seperate_Field_And_Alias()
        {
            // Arrange
            var expected = "{\"query\":\"query{field}\"}";
            var fieldBuilder = new GraphQLFieldBuilder();
            var queryBuilder = new GraphQLQueryGeneratorFromFields();

            // Act
            // Get fields
            var fields =
                fieldBuilder.GetFields(typeof(Query_Test_Single_Field_Should_Not_Generate_Seperate_Field_And_Alias));
            var actual = queryBuilder.GetQuery(fields);

            // Assert
            Assert.Equal(expected, actual);
        }

        public class Query_Test_Single_Field_Should_Not_Generate_Seperate_Field_And_Alias
        {
            public string Field { get; set; }
        }

        [Fact]
        public void Test_Case_Hero()
        {
            // Arrange
            var expected = "{\"query\":\"query{hero{friends{name} name}}\"}";
            var fieldBuilder = new GraphQLFieldBuilder();
            var queryBuilder = new GraphQLQueryGeneratorFromFields();

            // Act
            // Get fields
            var fields =
                fieldBuilder.GetFields(typeof(Query_Test_Case_Hero));
            var actual = queryBuilder.GetQuery(fields);

            // Assert
            Assert.Equal(expected, actual);
        }

        public class Query_Test_Case_Hero
        {
            public Query_Test_Case_Hero_Hero Hero { get; set; }
        }

        public class Query_Test_Case_Hero_Hero : Query_Test_Case_Hero_Person
        {
            public IEnumerable<Query_Test_Case_Hero_Person> Friends { get; set; }
        }

        public class Query_Test_Case_Hero_Person
        {
            public string Name { get; set; }
        }
    }
}
