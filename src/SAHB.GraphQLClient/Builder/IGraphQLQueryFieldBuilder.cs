namespace SAHB.GraphQLClient.Builder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Contains methods for building a GraphQL field
    /// </summary>
    public interface IGraphQLQueryFieldBuilder : IGraphQLBuilder
    {
        /// <summary>
        /// Sets the alias of the field
        /// </summary>
        /// <param name="alias">The alias to use for the field</param>
        /// <returns>Returns the same instance</returns>
        IGraphQLQueryFieldBuilder Alias(string alias);

        /// <summary>
        /// Adds a argument to the field
        /// </summary>
        /// <param name="argumentName">The argument name used on the GraphQL server</param>
        /// <param name="argumentType">The argument type used on the GraphQL server, for example Int or String</param>
        /// <param name="variableName">The variable used to execute the query</param>
        /// <returns>Returns the same instance</returns>
        IGraphQLQueryFieldBuilder Argument(string argumentName, string argumentType, string variableName);

        /// <summary>
        /// Adds a argument to the field
        /// </summary>
        /// <param name="argumentName">The argument name used on the GraphQL server</param>
        /// <param name="argumentType">The argument type used on the GraphQL server, for example Int or String</param>
        /// <param name="variableName">The variable used to execute the query</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        /// <returns>Returns the same instance</returns>
        IGraphQLQueryFieldBuilder Argument(string argumentName, string argumentType, string variableName, bool isRequired);

        /// <summary>
        /// Adds a argument to the field
        /// </summary>
        /// <param name="argumentName">The argument name used on the GraphQL server</param>
        /// <param name="argumentType">The argument type used on the GraphQL server, for example Int or String</param>
        /// <param name="variableName">The variable used to execute the query</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        /// <param name="inlineArgument">Should the GraphQL argument be inlined</param>
        /// <returns>Returns the same instance</returns>
        IGraphQLQueryFieldBuilder Argument(string argumentName, string argumentType, string variableName, bool isRequired, bool? inlineArgument);

        /// <summary>
        /// Adds a argument to the field
        /// </summary>
        /// <param name="argumentName">The argument name used on the GraphQL server</param>
        /// <param name="argumentType">The argument type used on the GraphQL server, for example Int or String</param>
        /// <param name="variableName">The variable used to execute the query</param>
        /// <param name="isRequired">Is the GraphQL argument required to execute the query</param>
        /// <param name="inlineArgument">Should the GraphQL argument be inlined</param>
        /// <param name="defaultValue">The default value for the GraphQL argument</param>
        /// <returns>Returns the same instance</returns>
        IGraphQLQueryFieldBuilder Argument(string argumentName, string argumentType, string variableName, bool isRequired, bool? inlineArgument, object defaultValue);
    }
}