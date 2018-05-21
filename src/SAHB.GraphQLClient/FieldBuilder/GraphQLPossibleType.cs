using System;

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
        /// <param name="type">The type of the possible type</param>
        /// <param name="typeName">The name which should be returned from the __typename field defined in the GraphQL draft from Oct 2016</param>
        public GraphQLPossibleType(Type type, string typeName)
        {
            Type = type;
            TypeName = typeName;
        }

        /// <summary>
        /// The possible return type, this should inherit from the field
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The typename which is returned from the __typename defined in the GraphQL draft from Oct 2016
        /// </summary>
        public string TypeName { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return TypeName + ": " + Type.FullName;
        }
    }
}