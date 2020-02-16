using System;
using System.Collections.Generic;
using System.Linq;
using SAHB.GraphQL.Client.Introspection.Validation;

namespace SAHB.GraphQLClient.Exceptions
{
    /// <summary>
    /// Exception is thrown if any validation error of the query type was detected
    /// </summary>
    public class GraphQLValidationErrorException : GraphQLException
    {
        /// <summary>
        /// The validation errors for the query type
        /// </summary>
        public IEnumerable<ValidationError> ValidationErrors;

        public GraphQLValidationErrorException(IEnumerable<ValidationError> validationErrors) : base(GetMessage(validationErrors))
        {
            this.ValidationErrors = validationErrors;
        }

        private static string GetMessage(IEnumerable<ValidationError> validationErrors)
        {
            return string.Join(
                Environment.NewLine,
                validationErrors.Select(e => e.ToString()));
        }
    }
}
