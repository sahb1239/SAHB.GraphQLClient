using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Attribute which defines a skip argument to support skipping a number of results. This could for example be used for paging
    /// </summary>    
    /// <remarks>
    /// Note: Skip should always be applied before take, since Skip else would not be useful
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class GraphQLSkipArgumentsAttribute : GraphQLArgumentsAttribute
    {
        /// <summary>
        /// Initializes the skip argument attribute using the argumentName "skip" and the argumentType "Int"
        /// </summary>
        /// <param name="variableName">The variable name to use in the query</param>
        public GraphQLSkipArgumentsAttribute(string variableName) : base("skip", "Int", variableName)
        {
        }

        /// <summary>
        /// Initializes a skip argument attribute which defines a argument which is used for a GraphQL field
        /// </summary>
        /// <param name="argumentName">The argument name used in the GraphQL query</param>
        /// <param name="argumentType">The argument type of the argument in the GraphQL query</param>
        /// <param name="variableName">The variable name used in the GraphQL query</param>
        [Obsolete]
        public GraphQLSkipArgumentsAttribute(string argumentName, string argumentType, string variableName) : base(argumentName, argumentType, variableName)
        {
        }
    }
}
