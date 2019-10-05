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
        /// Initializes the last argument attribute using the argumentName "last" and the argumentType "Int"
        /// </summary>
        /// <param name="variableName">The variable name to use in the query</param>
        public GraphQLLastArgumentsAttribute(string variableName) : base("last", "Int", variableName)
        {
        }
    }
}
