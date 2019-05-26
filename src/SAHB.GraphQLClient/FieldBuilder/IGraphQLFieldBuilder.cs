using SAHB.GraphQL.Client.FieldBuilder;
using System;
using System.Collections.Generic;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Generates a selectionSet <see cref="IEnumerable{T}"/> of <see cref="IGraphQLField"/> which represents the selection from a given <see cref="Type"/>
    /// </summary>
    public interface IGraphQLFieldBuilder
    {
        /// <summary>
        /// Generates a selectionSet for a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The type which to generate the operation from</param>
        /// <returns>The selectionSet</returns>
        IEnumerable<IGraphQLField> GenerateSelectionSet(Type type);

        /// <summary>
        /// Generates a <see cref="IEnumerable{T}"/> of <see cref="GraphQLField"/> which contains metadata from a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The type which to generate the fields from</param>
        /// <returns>The metadata from the type</returns>
        [Obsolete("Please use GenerateSelectionSet instead")]
        IEnumerable <IGraphQLField> GetFields(Type type);
    }
}
