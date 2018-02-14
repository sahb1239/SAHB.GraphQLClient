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

        public GraphQLHttpExecutorServerErrorStatusCodeException(HttpStatusCode statusCode, string query, string response, string message) : base(GetMessage(statusCode, query, response, message))
        {
            StatusCode = statusCode;
            Query = query;
            Response = response;
        }

        public GraphQLHttpExecutorServerErrorStatusCodeException(HttpStatusCode statusCode, string query, string response, string message, Exception innerException) : base(GetMessage(statusCode, query, response, message), innerException)
        {
            StatusCode = statusCode;
            Query = query;
            Response = response;
        }

        private static string GetMessage(HttpStatusCode statusCode, string query, string response, string message)
        {
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrEmpty(message))
                builder.AppendLine(message);
            if (!string.IsNullOrEmpty(query))
                builder.AppendLine("Query: " + query);
            builder.AppendLine("StatusCode: " + statusCode);
            if (!string.IsNullOrEmpty(response))
                builder.AppendLine("Response: " + response);
            return builder.ToString().TrimEnd();
        }
    }
}
