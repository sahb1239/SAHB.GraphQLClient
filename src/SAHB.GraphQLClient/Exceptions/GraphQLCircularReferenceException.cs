using System;
using System.Collections.Generic;
using System.Linq;

namespace SAHB.GraphQLClient.Exceptions
{
    /// <summary>
    /// Exception thrown when á circular reference is found
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class GraphQLCircularReferenceException : GraphQLException
    {
        public GraphQLCircularReferenceException(IEnumerable<Type> types) 
            : base($"Circular reference found for the following types:{Environment.NewLine}{string.Join(" -> ", types.Select(type => type.FullName).ToArray())}")
        {
            Types = types;
        }

        public IEnumerable<Type> Types { get; }
    }
}
