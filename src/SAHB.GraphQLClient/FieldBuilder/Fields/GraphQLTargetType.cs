using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAHB.GraphQLClient.FieldBuilder
{
    /// <inheritdoc />
    public class GraphQLTargetType
    {
        public GraphQLTargetType(Type type, IEnumerable<GraphQLField> selectionSet)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            SelectionSet = selectionSet?.ToList() ?? throw new ArgumentNullException(nameof(selectionSet));
        }

        /// <inheritdoc />
        public Type Type { get; }

        /// <inheritdoc />
        public ICollection<GraphQLField> SelectionSet { get; }
    }
}
