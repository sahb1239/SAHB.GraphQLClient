using System;

namespace SAHB.GraphQLClient.Exceptions
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Exception which all GraphQL exceptions inherits from
    /// </summary>
    public abstract class GraphQLException : Exception
    {
        protected GraphQLException()
        {
        }

        protected GraphQLException(string message) : base(message)
        {
        }

        protected GraphQLException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
