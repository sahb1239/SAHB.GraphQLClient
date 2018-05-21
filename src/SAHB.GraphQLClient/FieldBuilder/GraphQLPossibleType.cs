using System;
using System.Collections.Generic;
using System.Linq;

namespace SAHB.GraphQLClient.FieldBuilder
{
    /// <summary>
    /// Contains the possible types for the GraphQL field. This is used for unions and interfaces
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class GraphQLPossibleType
    {
        /// <summary>
        /// Initilizes a GraphQL possible type used for unions and interfaces
        /// </summary>
        /// <param name="fields">The fields for the possible type</param>
        /// <param name="typeName">The name which should be returned from the __typename field defined in the GraphQL draft from Oct 2016</param>
        public GraphQLPossibleType(IEnumerable<GraphQLField> fields, string typeName)
        {
            Fields = (fields ?? Enumerable.Empty<GraphQLField>()).ToList();
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        /// <summary>
        /// The fields for the GraphQL possible types
        /// </summary>
        public ICollection<GraphQLField> Fields { get; }

        /// <summary>
        /// The typename which is returned from the __typename defined in the GraphQL draft from Oct 2016
        /// </summary>
        public string TypeName { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return TypeName + ": " +
                   Environment.NewLine + "   " + string.Join(Environment.NewLine + "   ", Fields);
        }
    }
}