using System;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.QueryGenerator
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Builds a GraphQL query or mutation from the specified <see cref="Type"/> and the <see cref="GraphQLQueryArgument"/>s
    /// </summary>
    public interface IGraphQLQueryGenerator
    {
        /// <summary>
        /// Builds a GraphQL query from the specified <see cref="Type"/> and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <param name="type">The type to generate the query from</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated query</returns>
        string GetQuery(Type type, params GraphQLQueryArgument[] arguments);

        /// <summary>
        /// Builds a GraphQL mutation from the specified <see cref="Type"/> and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <param name="type">The type to generate the mutation from</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated mutation</returns>
        string GetMutation(Type type, params GraphQLQueryArgument[] arguments);
    }
}