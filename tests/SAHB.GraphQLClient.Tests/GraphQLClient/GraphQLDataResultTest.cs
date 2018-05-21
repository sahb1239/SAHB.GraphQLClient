using System;
using System.Collections.Generic;
using System.Text;
using SAHB.GraphQLClient.Result;
using Xunit;

namespace SAHB.GraphQLClient.Tests.GraphQLClient
{
    public class GraphQLDataResultTest
    {
        [Fact]
        public void GraphQLDataResult_Should_Return_False_On_Data_And_Errors()
        {
            // Arrange
            var result = new GraphQLDataResult<string>();

            // Act / Assert
            Assert.False(result.ContainsData);
            Assert.False(result.ContainsErrors);
        }

        [Fact]
        public void GraphQLDataResult_Should_Return_True_On_Data_When_Contains_Data()
        {
            // Arrange
            var result = new GraphQLDataResult<string>
            {
                Data = ""
            };

            // Act / Assert
            Assert.True(result.ContainsData);
            Assert.False(result.ContainsErrors);
        }

        [Fact]
        public void GraphQLDataResult_Should_Return_True_On_Errors_When_Contains_Errors()
        {
            // Arrange
            var result = new GraphQLDataResult<string>
            {
                Errors = new List<GraphQLDataError>
                {
                    new GraphQLDataError()
                }
            };
            
            // Act / Assert
            Assert.True(result.ContainsErrors);
            Assert.False(result.ContainsData);
        }
    }
}
