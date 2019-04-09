using System;
using System.Collections.Generic;

namespace SAHB.GraphQLClient
{
    public interface IGraphQLTargetType
    {
        /// <summary>
        /// The type to deserilize to
        /// </summary>
        Type Type { get; }
        
        /// <summary>
        /// The selection set for the type
        /// </summary>
        ICollection<IGraphQLField> SelectionSet { get; }
    }
}