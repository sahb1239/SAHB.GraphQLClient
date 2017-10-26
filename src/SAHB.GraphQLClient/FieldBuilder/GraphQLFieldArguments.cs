using System;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL argument used to contain metadata which can be used for generating a GraphQL query
    /// </summary>
    public class GraphQLFieldArguments
    {
        /// <summary>
        /// Initilizes a GraphQL argument used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="argumentName">GraphQL argument name</param>
        /// <param name="variableName">GraphQL variable name</param>
        public GraphQLFieldArguments(string argumentName, string variableName)
        {
            ArgumentName = argumentName ?? throw new ArgumentNullException(nameof(argumentName));
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
        }

        /// <summary>
        /// GraphQL argument name
        /// </summary>
        public string ArgumentName { get; set; }

        /// <summary>
        /// GraphQL variable name
        /// </summary>
        public string VariableName { get; set; }
    }
}