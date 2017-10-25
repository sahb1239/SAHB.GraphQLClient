using System;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.QueryBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Extentions for the <see cref="IGraphQLQueryBuilder"/> interface
    /// </summary>
    public static class GraphQLQueryBuilderExtentions
    {
        /// <summary>
        /// Builds a GraphQL query from the specified <see cref="Type"/> and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="queryBuilder">The queryBuilder used to generate the query</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated query</returns>
        public static string GetQuery<T>(this IGraphQLQueryBuilder queryBuilder,
            params GraphQLQueryArgument[] arguments)
        {
            if (queryBuilder == null) throw new ArgumentNullException(nameof(queryBuilder));
            return queryBuilder.GetQuery(typeof(T), arguments);
        }

        /// <summary>
        /// Builds a GraphQL mutation from the specified <see cref="Type"/> and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <typeparam name="T">The type to generate the mutation from</typeparam>
        /// <param name="queryBuilder">The queryBuilder used to generate the query</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated mutation</returns>
        public static string GetMutation<T>(this IGraphQLQueryBuilder queryBuilder,
            params GraphQLQueryArgument[] arguments)
        {
            if (queryBuilder == null) throw new ArgumentNullException(nameof(queryBuilder));
            return queryBuilder.GetMutation(typeof(T), arguments);
        }
    }
}
