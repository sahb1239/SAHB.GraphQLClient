namespace SAHB.GraphQLClient
{
    /// <summary>
    /// GraphQL argument used to contain metadata which can be used for generating a GraphQL query
    /// </summary>
    public interface IGraphQLArguments
    {
        /// <summary>
        /// GraphQL argument name
        /// </summary>
        string ArgumentName { get; }

        /// <summary>
        /// The argument type of the variable
        /// </summary>
        string ArgumentType { get; }
        
        /// <summary>
        /// GraphQL variable name
        /// </summary>
        string VariableName { get; set; }

        /// <summary>
        /// Default value for the argument if not set using variable
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// Is the argument required for execution of the query
        /// </summary>
        bool IsRequired { get; set; }

        /// <summary>
        /// Should the argument be inlined in the query
        /// </summary>
        bool? InlineArgument { get; set; }
    }
}