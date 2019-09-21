using System.Collections.Generic;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;

namespace SAHB.GraphQL.Client.Introspection.Validation
{
    /// <summary>
    /// GraphQL validator which supports validating if the specified selectionSet is valid for a given <see cref="GraphQLIntrospectionSchema"/> and the given <see cref="GraphQLOperationType"/>
    /// </summary>
    public interface IGraphQLValidation
    {
        /// <summary>
        /// Validate if the GraphQL <paramref name="selectionSet"/> is valid for the specified <paramref name="graphQLIntrospectionSchema"/> and <paramref name="operationType"/>
        /// </summary>
        /// <param name="graphQLIntrospectionSchema">The introspectionSchema to validate against</param>
        /// <param name="operationType">The operationType to validate against</param>
        /// <param name="selectionSet">The selectionSet which should be validated</param>
        /// <returns>An empty list if no errors were found or a <see cref="ValidationError"/> for each error found</returns>
        IEnumerable<ValidationError> ValidateGraphQLSelectionSet(GraphQLIntrospectionSchema graphQLIntrospectionSchema, GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet);
    }
}