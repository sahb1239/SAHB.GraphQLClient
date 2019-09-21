using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAHB.GraphQL.Client.Introspection.Validation
{
    public static class GraphQLValidationExtentions
    {
        /// <summary>
        /// Validate if the GraphQL query type <typeparamref name="T"/> is valid for the specified <paramref name="graphQLIntrospectionQuery"/> and <paramref name="operationType"/>
        /// </summary>
        /// <typeparam name="T">The GraphQL query type to validate against</typeparam>
        /// <param name="graphQLIntrospectionQuery">The introspectionSchema to validate against</param>
        /// <param name="operationType">The operationType to validate against</param>
        /// <returns>An empty list if no errors were found or a <see cref="ValidationError"/> for each error found</returns>
        public static IEnumerable<ValidationError> ValidateGraphQLType<T>(this GraphQLIntrospectionQuery graphQLIntrospectionQuery, GraphQLOperationType operationType)
        {
            if (graphQLIntrospectionQuery is null)
            {
                throw new ArgumentNullException(nameof(graphQLIntrospectionQuery));
            }

            return ValidateGraphQLType<T>(graphQLIntrospectionQuery.Schema, operationType);
        }

        /// <summary>
        /// Validate if the GraphQL query type <typeparamref name="T"/> is valid for the specified <paramref name="graphQLIntrospectionSchema"/> and <paramref name="operationType"/>
        /// </summary>
        /// <typeparam name="T">The GraphQL query type to validate against</typeparam>
        /// <param name="graphQLIntrospectionSchema">The introspectionSchema to validate against</param>
        /// <param name="operationType">The operationType to validate against</param>
        /// <returns>An empty list if no errors were found or a <see cref="ValidationError"/> for each error found</returns>
        public static IEnumerable<ValidationError> ValidateGraphQLType<T>(this GraphQLIntrospectionSchema graphQLIntrospectionSchema, GraphQLOperationType operationType)
        {
            return ValidateGraphQLType<T>(graphQLIntrospectionSchema, operationType, new GraphQLFieldBuilder());
        }

        private static IEnumerable<ValidationError> ValidateGraphQLType<T>(this GraphQLIntrospectionSchema graphQLIntrospectionSchema, GraphQLOperationType operationType, IGraphQLFieldBuilder fieldBuilder)
        {
            if (fieldBuilder is null)
            {
                throw new ArgumentNullException(nameof(fieldBuilder));
            }

            var validator = new GraphQLValidation();
            return validator.ValidateGraphQLSelectionSet(graphQLIntrospectionSchema, operationType, fieldBuilder.GenerateSelectionSet(typeof(T)));
        }
    }
}
