using System;
using System.Linq.Expressions;
using SAHB.GraphQLClient.Filtering;
using Xunit;

namespace SAHB.GraphQLClient.Tests.Filtering
{
    public class ExpressionHelperTests
    {
        [Fact]
        public void Single_Member_Name()
        {
            // Arrange
            Expression<Func<GraphQLQuery, object>> expression = e => new GraphQLQuery
            {
                Field2 = e.Field2
            };

            // Act
            var memberNames = ExpressionHelper.GetMemberNamesFromExpression(expression);

            // Assert
            var memberName = Assert.Single(memberNames);
            Assert.Equal("Field2", memberName);
        }

        [Fact]
        public void Single_Member_Name_Mapping()
        {
            // Arrange
            Expression<Func<GraphQLQuery, object>> expression = e => new GraphQLQuery
            {
                Field2 = e.Field2
            };

            // Act
            var memberNames = ExpressionHelper.GetMemberNamesFromExpression(expression);

            // Assert
            var memberName = Assert.Single(memberNames);
            Assert.Equal("Field2", memberName);
        }

        [Fact]
        public void Nested_Member_Name()
        {
            // Arrange
            Expression<Func<GraphQLQuery, object>> expression = e => new GraphQLQuery
            {
                Field = new GraphQLQuery
                {
                    Field2 = e.Field.Field2
                }
            };

            // Act
            var memberNames = ExpressionHelper.GetMemberNamesFromExpression(expression);

            // Assert
            var memberName = Assert.Single(memberNames);
            Assert.Equal("Field.Field2", memberName);
        }

        [Fact]
        public void Single_Member_Name_Mapping_From_Nested()
        {
            // Arrange
            Expression<Func<GraphQLQuery, object>> expression = e => new GraphQLQuery
            {
                Field2 = e.Field.Field2
            };

            // Act
            var memberNames = ExpressionHelper.GetMemberNamesFromExpression(expression);

            // Assert
            var memberName = Assert.Single(memberNames);
            Assert.Equal("Field.Field2", memberName);
        }

        [Fact]
        public void Anonomous_Type_Single_Member_Name()
        {
            // Arrange
            Expression<Func<GraphQLQuery, object>> expression = e => new
            {
                OtherFieldName = e.Field2
            };

            // Act
            var memberNames = ExpressionHelper.GetMemberNamesFromExpression(expression);

            // Assert
            var memberName = Assert.Single(memberNames);
            Assert.Equal("Field2", memberName);
        }

        [Fact]
        public void Anonomous_Type_Single_Member_Name_Mapping()
        {
            // Arrange
            Expression<Func<GraphQLQuery, object>> expression = e => new
            {
                OtherFieldName = e.Field2
            };

            // Act
            var memberNames = ExpressionHelper.GetMemberNamesFromExpression(expression);

            // Assert
            var memberName = Assert.Single(memberNames);
            Assert.Equal("Field2", memberName);
        }

        [Fact]
        public void Test_Nested_Member_Name()
        {
            // Arrange
            Expression<Func<GraphQLQuery, object>> expression = e => new
            {
                OtherFieldName = new GraphQLQuery
                {
                    Field2 = e.Field.Field2
                }
            };

            // Act
            var memberNames = ExpressionHelper.GetMemberNamesFromExpression(expression);

            // Assert
            var memberName = Assert.Single(memberNames);
            Assert.Equal("Field.Field2", memberName);
        }

        [Fact]
        public void Test_Single_Member_Name_Mapping_From_Nested()
        {
            // Arrange
            Expression<Func<GraphQLQuery, object>> expression = e => new
            {
                OtherFieldName = e.Field.Field2
            };

            // Act
            var memberNames = ExpressionHelper.GetMemberNamesFromExpression(expression);

            // Assert
            var memberName = Assert.Single(memberNames);
            Assert.Equal("Field.Field2", memberName);
        }

        class GraphQLQuery
        {
            public GraphQLQuery Field { get; set; }
            public string Field2 { get; set; }
        }
    }
}
