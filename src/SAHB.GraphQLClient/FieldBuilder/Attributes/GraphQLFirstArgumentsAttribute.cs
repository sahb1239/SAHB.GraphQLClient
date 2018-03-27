using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{  
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Attribute which defines a argument to taking a specified first number of results. This is typically used with GraphQL connections
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class GraphQLFirstArgumentsAttribute : GraphQLArgumentsAttribute
    {
        /// <summary>
        /// Initilizes the first argument attribute using the argumentName "first" and the argumentType "Int"
        /// </summary>
        /// <param name="variableName">The variable name to use in the query</param>
        public GraphQLFirstArgumentsAttribute(string variableName) : base("first", "Int", variableName)
        {
        }

        /// <summary>
        /// Initilizes a first argument attribute which defines a argument which is used for a GraphQL field
        /// </summary>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        public GraphQLFirstArgumentsAttribute(string argumentName, string argumentType, string variableName) : base(argumentName, argumentType, variableName)
        {
        }
    }
}
