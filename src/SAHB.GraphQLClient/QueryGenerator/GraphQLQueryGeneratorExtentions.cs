using System;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.QueryGenerator
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Extentions for the <see cref="IGraphQLQueryGenerator"/> interface
    /// </summary>
    public static class GraphQLQueryGeneratorExtentions
    {
        /// <summary>
        /// Builds a GraphQL query from the specified <see cref="Type"/> and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="queryGenerator">The queryGenerator used to generate the query</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated query</returns>
        public static string GetQuery<T>(this IGraphQLQueryGenerator queryGenerator,
            params GraphQLQueryArgument[] arguments)
        {
            if (queryGenerator == null) throw new ArgumentNullException(nameof(queryGenerator));
            return queryGenerator.GetQuery(typeof(T), arguments);
        }

        /// <summary>
        /// Builds a GraphQL mutation from the specified <see cref="Type"/> and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <typeparam name="T">The type to generate the mutation from</typeparam>
        /// <param name="queryGenerator">The queryGenerator used to generate the query</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated mutation</returns>
        public static string GetMutation<T>(this IGraphQLQueryGenerator queryGenerator,
            params GraphQLQueryArgument[] arguments)
        {
            if (queryGenerator == null) throw new ArgumentNullException(nameof(queryGenerator));
            return queryGenerator.GetMutation(typeof(T), arguments);
        }
    }
}
