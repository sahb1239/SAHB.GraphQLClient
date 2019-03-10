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
        /// Initializes the first argument attribute using the argumentName "first" and the argumentType "Int"
        /// </summary>
        /// <param name="variableName">The variable name to use in the query</param>
        public GraphQLFirstArgumentsAttribute(string variableName) : base("first", "Int", variableName)
        {
        }
    }
}
