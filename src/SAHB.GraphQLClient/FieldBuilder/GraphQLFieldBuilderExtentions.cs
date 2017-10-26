using System;
using System.Collections.Generic;
using System.Text;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Extentions for the <see cref="IGraphQLFieldBuilder"/> interface
    /// </summary>
    public static class GraphQLFieldBuilderExtentions
    {
        /// <summary>
        /// Generates a <see cref="IEnumerable{T}"/> of <see cref="GraphQLField"/> which contains metadata from a given typeparameter <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">The type which to generate the fields from</typeparam>
        /// <param name="fieldBuilder">The fieldbuilder to use</param>
        /// <returns>The metadata from the type</returns>
        public static IEnumerable<GraphQLField> GetFields<T>(this IGraphQLFieldBuilder fieldBuilder)
        {
            if (fieldBuilder == null) throw new ArgumentNullException(nameof(fieldBuilder));
            return fieldBuilder.GetFields(typeof(T));
        }
    }
}
