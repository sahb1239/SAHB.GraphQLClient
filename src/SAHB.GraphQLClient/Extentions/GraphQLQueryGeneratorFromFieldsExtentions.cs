using System;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Extentions
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Extensions for the <see cref="IGraphQLQueryGeneratorFromFields"/> interface
    /// </summary>
    public static class GraphQLQueryGeneratorFromFieldsExtentions
    {
        /// <summary>
        /// Builds a GraphQL query from the specified <see cref="Type"/> and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <typeparam name="T">The type to generate the query from</typeparam>
        /// <param name="queryGenerator">The queryGenerator used to generate the query</param>
        /// <param name="fieldBuilder">The fieldBuilder used for examining the type</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated query</returns>
        public static string GetQuery<T>(this IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLFieldBuilder fieldBuilder,
            params GraphQLQueryArgument[] arguments)
        {
            if (queryGenerator == null) throw new ArgumentNullException(nameof(queryGenerator));
            if (fieldBuilder == null) throw new ArgumentNullException(nameof(fieldBuilder));
            var selectionSet = fieldBuilder.GenerateSelectionSet(typeof(T));
            return queryGenerator.GenerateQuery(GraphQLOperationType.Query, selectionSet, arguments);
        }

        /// <summary>
        /// Builds a GraphQL mutation from the specified <see cref="Type"/> and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <typeparam name="T">The type to generate the mutation from</typeparam>
        /// <param name="queryGenerator">The queryGenerator used to generate the query</param>
        /// <param name="fieldBuilder">The fieldBuilder used for examining the type</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated mutation</returns>
        public static string GetMutation<T>(this IGraphQLQueryGeneratorFromFields queryGenerator, IGraphQLFieldBuilder fieldBuilder,
            params GraphQLQueryArgument[] arguments)
        {
            if (queryGenerator == null) throw new ArgumentNullException(nameof(queryGenerator));
            if (fieldBuilder == null) throw new ArgumentNullException(nameof(fieldBuilder));
            var selectionSet = fieldBuilder.GenerateSelectionSet(typeof(T));
            return queryGenerator.GenerateQuery(GraphQLOperationType.Mutation, selectionSet, arguments);
        }
    }
}
