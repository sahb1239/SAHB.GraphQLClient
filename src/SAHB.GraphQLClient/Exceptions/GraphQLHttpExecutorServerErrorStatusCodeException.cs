using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SAHB.GraphQLClient.Exceptions
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Throws a new <see cref="GraphQLHttpExecutorServerErrorStatusCodeException"/> which indicates that the GraphQL request returned a non successfull server status code
    /// </summary>
    public class GraphQLHttpExecutorServerErrorStatusCodeException : GraphQLException
    {
        public HttpStatusCode StatusCode { get; }
        public string Query { get; }
        public string Response { get; }

        public GraphQLHttpExecutorServerErrorStatusCodeException(HttpStatusCode statusCode, string query, string response, string message) : base(message)
        {
            StatusCode = statusCode;
            Query = query;
            Response = response;
        }

        public GraphQLHttpExecutorServerErrorStatusCodeException(HttpStatusCode statusCode, string query, string response, string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
            Query = query;
            Response = response;
        }
    }
}
