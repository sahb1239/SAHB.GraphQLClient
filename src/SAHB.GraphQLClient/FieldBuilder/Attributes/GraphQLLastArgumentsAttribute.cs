using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Attribute which defines a argument to taking a specified last number of results. This is typically used with GraphQL connections
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class GraphQLLastArgumentsAttribute : GraphQLArgumentsAttribute
    {
        /// <summary>
        /// Initilizes the last argument attribute using the argumentName "last" and the argumentType "Int"
        /// </summary>
        /// <param name="variableName">The variable name to use in the query</param>
        public GraphQLLastArgumentsAttribute(string variableName) : base("last", "Int", variableName)
        {
        }

        /// <summary>
        /// Initilizes a last argument attribute which defines a argument which is used for a GraphQL field
        /// </summary>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        public GraphQLLastArgumentsAttribute(string argumentName, string argumentType, string variableName) : base(argumentName, argumentType, variableName)
        {
        }
    }
}