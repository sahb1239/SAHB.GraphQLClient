using System;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.QueryBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL argument used to contain variable value and type of a argument which is added to a query
    /// </summary>
    public class GraphQLQueryArgument
    {
        /// <summary>
        /// Initilizes a GraphQL argument used to contain variable value and type of a argument which is added to a query
        /// </summary>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        public GraphQLQueryArgument(string variableName, string argumentValue)
        {
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            ArgumentValue = argumentValue ?? throw new ArgumentNullException(nameof(argumentValue));
        }

        /// <summary>
        /// The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/>
        /// </summary>
        public string VariableName { get; set; }

        /// <summary>
        /// The value which is inserted in the variables part of the GraphQL query
        /// </summary>
        public string ArgumentValue { get; set; }
    }
}