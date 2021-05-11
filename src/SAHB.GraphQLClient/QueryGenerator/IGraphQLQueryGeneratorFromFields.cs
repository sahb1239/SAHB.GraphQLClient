﻿using System;
using System.Collections.Generic;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.QueryGenerator
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Builds a GraphQL query or mutation from the specified <see cref="GraphQLField"/>s and the <see cref="GraphQLQueryArgument"/>s
    /// </summary>
    public interface IGraphQLQueryGeneratorFromFields
    {
        /// <summary>
        /// Builds a GraphQL query from the specified <see cref="GraphQLField"/>s and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <param name="fields">The GraphQL fields to generate the query from</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated query</returns>
        [Obsolete("Please use GenerateQuery instead")]
        string GetQuery(IEnumerable<GraphQLField> fields, params GraphQLQueryArgument[] arguments);

        /// <summary>
        /// Builds a GraphQL mutation from the specified <see cref="GraphQLField"/>s and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <param name="fields">The GraphQL fields to generate the mutation from</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated mutation</returns>
        [Obsolete("Please use GenerateQuery instead")]
        string GetMutation(IEnumerable<GraphQLField> fields, params GraphQLQueryArgument[] arguments);

        /// <summary>
        /// Builds a GraphQL query from the specified <see cref="IGraphQLOperation"/>s and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <param name="operationType">The GraphQL operation to generate the query from</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated query</returns>
        string GenerateQuery(GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet, params GraphQLQueryArgument[] arguments);

        /// <summary>
        /// Builds a GraphQL query from the specified <see cref="IGraphQLOperation"/>s and the <see cref="GraphQLQueryArgument"/>s
        /// </summary>
        /// <param name="operationType">The GraphQL operation to generate the query from</param>
        /// <param name="operationName">The operation name of the query, can be null if no operation name needs to be set</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <returns>The generated query</returns>
        string GenerateQuery(GraphQLOperationType operationType, string operationName, IEnumerable<GraphQLField> selectionSet, params GraphQLQueryArgument[] arguments);
        
        /// <summary>
        /// Builds a GraphQL query from the specified <see cref="IGraphQLOperation"/>s and the <see cref="GraphQLQueryArgument"/>s and the specified filter used to select the required fields
        /// </summary>
        /// <param name="operationType">The GraphQL operation to generate the query from</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="filter">The filter for the fields. If a field is included which has subfields, but no subfields are included all subfields are included</param>
        /// <returns>The generated query</returns>
        string GenerateQuery(GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet, Func<GraphQLField, bool> filter, params GraphQLQueryArgument[] arguments);
        
        /// <summary>
        /// Builds a GraphQL query from the specified <see cref="IGraphQLOperation"/>s and the <see cref="GraphQLQueryArgument"/>s and the specified filter used to select the required fields
        /// </summary>
        /// <param name="operationType">The GraphQL operation to generate the query from</param>
        /// <param name="operationName">The operation name of the query, can be null if no operation name needs to be set</param>
        /// <param name="arguments">The argument values which is inserted using a variable on specified arguments with the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="filter">The filter for the fields. If a field is included which has subfields, but no subfields are included all subfields are included</param>
        /// <returns>The generated query</returns>
        string GenerateQuery(GraphQLOperationType operationType, string operationName, IEnumerable<GraphQLField> selectionSet, Func<GraphQLField, bool> filter, params GraphQLQueryArgument[] arguments);
    }
}
