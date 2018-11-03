using SAHB.GraphQL.Client.FieldBuilder;
using System;
using System.Collections.Generic;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Generates a <see cref="IEnumerable{T}"/> of <see cref="GraphQLField"/> which contains metadata from a given <see cref="Type"/>
    /// </summary>
    public interface IGraphQLFieldBuilder
    {
        /// <summary>
        /// Generates a <see cref="IGraphQLOperation"/> for a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The type which to generate the operation from</param>
        /// <param name="operationType">The operation type to generate</param>
        /// <returns>A <see cref="IGraphQLOperation"/></returns>
        IGraphQLOperation GenerateOperation(Type type, GraphQLOperationType operationType);
    }
}
