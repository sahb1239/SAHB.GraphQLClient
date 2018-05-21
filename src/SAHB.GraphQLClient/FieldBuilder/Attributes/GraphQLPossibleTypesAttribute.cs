using System;
using System.Collections.Generic;
using System.Linq;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    /// <summary>
    /// Contains a GraphQL union type which supports deserilizing to different types based on the concrete GraphQL type
    /// </summary>
    // ReSharper disable once InconsistentNaming
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class GraphQLPossibleTypesAttribute : Attribute
    {
        /// <summary>
        /// Sets the possible types for this GraphQL field
        /// </summary>
        /// <param name="types">The possible types</param>
        public GraphQLPossibleTypesAttribute(params Type[] types)
        {
            PossibleTypes = types.ToList();
        }
        
        /// <summary>
        /// The possible types for the GraphQL field
        /// </summary>
        public ICollection<Type> PossibleTypes { get; }
    }
}
