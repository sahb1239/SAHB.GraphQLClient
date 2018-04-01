using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Attribute which defines a take argument to taking a specified number of results. This could for example be used for paging.
    /// </summary>    
    /// <remarks>
    /// Note: Skip should always be applied before take, since Skip else would not be useful
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class GraphQLTakeArgumentsAttribute : GraphQLArgumentsAttribute
    {
        /// <summary>
        /// Initilizes the take argument attribute using the argumentName "take" and the argumentType "Int"
        /// </summary>
        /// <param name="variableName">The variable name to use in the query</param>
        public GraphQLTakeArgumentsAttribute(string variableName) : base("take", "Int", variableName)
        {
        }

        /// <summary>
        /// Initilizes a take argument attribute which defines a argument which is used for a GraphQL field
        /// </summary>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        [Obsolete]
        public GraphQLTakeArgumentsAttribute(string argumentName, string argumentType, string variableName) : base(argumentName, argumentType, variableName)
        {
        }
    }
}