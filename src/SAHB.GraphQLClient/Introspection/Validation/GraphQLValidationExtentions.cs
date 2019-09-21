using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAHB.GraphQL.Client.Introspection.Validation
{
    public static class GraphQLValidationExtentions
    {
        public static IEnumerable<ValidationOutput> ValidateGraphQLType<T>(this GraphQLIntrospectionQuery graphQLIntrospectionQuery, GraphQLOperationType operationType)
        {
            if (graphQLIntrospectionQuery is null)
            {
                throw new ArgumentNullException(nameof(graphQLIntrospectionQuery));
            }

            return ValidateGraphQLType<T>(graphQLIntrospectionQuery.Schema, operationType);
        }

        public static IEnumerable<ValidationOutput> ValidateGraphQLType<T>(this GraphQLIntrospectionSchema graphQLIntrospectionSchema, GraphQLOperationType operationType)
        {
            return ValidateGraphQLType<T>(graphQLIntrospectionSchema, operationType, new GraphQLFieldBuilder());
        }

        public static IEnumerable<ValidationOutput> ValidateGraphQLType<T>(this GraphQLIntrospectionQuery graphQLIntrospectionQuery, GraphQLOperationType operationType, IGraphQLFieldBuilder fieldBuilder)
        {
            if (graphQLIntrospectionQuery is null)
            {
                throw new ArgumentNullException(nameof(graphQLIntrospectionQuery));
            }

            return ValidateGraphQLType<T>(graphQLIntrospectionQuery.Schema, operationType, fieldBuilder);
        }

        public static IEnumerable<ValidationOutput> ValidateGraphQLType<T>(this GraphQLIntrospectionSchema graphQLIntrospectionSchema, GraphQLOperationType operationType, IGraphQLFieldBuilder fieldBuilder)
        {
            if (fieldBuilder is null)
            {
                throw new ArgumentNullException(nameof(fieldBuilder));
            }

            return GraphQLValidation.ValidateGraphQLSelectionSet(graphQLIntrospectionSchema, operationType, fieldBuilder.GenerateSelectionSet(typeof(T)));
        }
    }
}
