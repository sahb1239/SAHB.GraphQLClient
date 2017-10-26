using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Attribute which defines a argument which is used for a GraphQL field
    /// </summary>    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class GraphQLArgumentsAttribute : Attribute
    {
        /// <summary>
        /// Initilizes a attribute which defines a argument which is used for a GraphQL field
        /// </summary>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        public GraphQLArgumentsAttribute(string argumentName, string variableName)
        {
            VariableName = variableName ?? throw new ArgumentException(nameof(variableName));
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
        }

        /// <summary>
        /// The argument name used in the GraphQL query
        /// </summary>
        public string ArgumentName { get; }

        /// <summary>
        /// The variable name used in the GraphQL query
        /// </summary>
        public string VariableName { get; }
    }
}