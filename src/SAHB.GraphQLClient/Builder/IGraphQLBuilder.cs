using System;

namespace SAHB.GraphQLClient.Builder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Contains methods for building a GraphQL query
    /// </summary>
    public interface IGraphQLBuilder
    {
        /// <summary>
        /// Creates a field in the query with the field name
        /// </summary>
        /// <param name="field">The graphQL field to request from the server</param>
        /// <returns>Returns the same instance</returns>
        IGraphQLBuilder Field(string field);

        /// <summary>
        /// Creates a field in the query with the field name
        /// </summary>
        /// <param name="field">The graphQL field to request from the server</param>
        /// <param name="generator">Configure the field</param>
        /// <returns>Returns the same instance</returns>
        IGraphQLBuilder Field(string field, Action<IGraphQLQueryFieldBuilder> generator);
    }
}