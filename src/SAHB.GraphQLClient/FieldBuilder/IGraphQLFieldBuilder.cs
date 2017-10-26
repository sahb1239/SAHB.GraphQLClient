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
        /// Generates a <see cref="IEnumerable{T}"/> of <see cref="GraphQLField"/> which contains metadata from a given <see cref="Type"/>
        /// </summary>
        /// <param name="type">The type which to generate the fields from</param>
        /// <returns>The metadata from the type</returns>
        IEnumerable<GraphQLField> GetFields(Type type);
    }
}
