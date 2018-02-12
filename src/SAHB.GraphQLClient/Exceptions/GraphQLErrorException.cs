using System;
using System.Collections.Generic;
using System.Linq;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Exceptions
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Throws a new <see cref="GraphQLErrorException"/> which indicates that the server returned one or more errors
    /// </summary>
    public class GraphQLErrorException : GraphQLException
    {
        public GraphQLErrorException(string query, IEnumerable<GraphQLDataError> errors) : this(query, errors, GetMessage(errors))
        {
        }

        public GraphQLErrorException(string query, IEnumerable<GraphQLDataError> errors, string message) : base(message)
        {
            Query = query;
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        private static string GetMessage(IEnumerable<GraphQLDataError> errors)
        {
            if (errors == null)
                return null;

            return string.Join(Environment.NewLine,
                errors.Select(error =>
                    $"{error.Message ?? "Error"} at {Environment.NewLine}{string.Join(Environment.NewLine, error.Locations?.Select(location => "   line: " + location.Line + " column: " + location.Column))}"));
        }

        public IEnumerable<GraphQLDataError> Errors { get; }
        public string Query { get; }
    }
}