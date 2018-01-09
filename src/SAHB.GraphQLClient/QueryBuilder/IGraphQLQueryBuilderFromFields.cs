using System.Collections.Generic;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.QueryBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Builds a GraphQL query or mutation from the specified <see cref="GraphQLField"/>s and the <see cref="GraphQLQueryArgument"/>s
    /// </summary>
    public interface IGraphQLQueryBuilderFromFields
    {
        /// <summary>
        /// Builds a GraphQL query from the specified <see cref="GraphQLField"/>s and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <param name="fields">The GraphQL fields to generate the query from</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated query</returns>
        string GetQuery(IEnumerable<GraphQLField> fields, params GraphQLQueryArgument[] arguments);

        /// <summary>
        /// Builds a GraphQL mutation from the specified <see cref="GraphQLField"/>s and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <param name="fields">The GraphQL fields to generate the mutation from</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated mutation</returns>
        string GetMutation(IEnumerable<GraphQLField> fields, params GraphQLQueryArgument[] arguments);
    }
}