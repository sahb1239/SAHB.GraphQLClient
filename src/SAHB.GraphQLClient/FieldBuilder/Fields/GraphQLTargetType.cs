using SAHB.GraphQLClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAHB.GraphQL.Client.FieldBuilder
{
    /// <inheritdoc />
    public class GraphQLTargetType : IGraphQLTargetType
    {
        public GraphQLTargetType(Type type, IEnumerable<IGraphQLField> selectionSet)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            SelectionSet = selectionSet?.ToList() ?? throw new ArgumentNullException(nameof(selectionSet));
        }

        /// <inheritdoc />
        public Type Type { get; }

        /// <inheritdoc />
        public ICollection<IGraphQLField> SelectionSet { get; }
    }
}
