using System;
using System.Collections.Generic;
using System.Text;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.Result;
using Xunit;

namespace SAHB.GraphQLClient.Tests.Exceptions
{

    public class GraphQLErrorExceptionTests
    {
        [Fact]
        public void Get_Message_Should_Not_Throw_Exception()
        {
            var errorList = new List<GraphQLDataError> { new GraphQLDataError { Message = "Access denied." } };

            new GraphQLErrorException("query", errorList);
        }
    }
}
