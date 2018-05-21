using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    /// <summary>
    /// Contains a GraphQL type name which is used for executing Union GraphQL fields
    /// </summary>
    // ReSharper disable once InconsistentNaming
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GraphQLTypeNameAttribute : Attribute
    {
        /// <summary>
        /// Override the default name of the GraphQL type used when requesting union or interface GraphQL types
        /// </summary>
        /// <param name="name">The name of the class used to select the correct type using the __typename defined in the GraphQL draft from Oct 2016</param>
        public GraphQLTypeNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// The name of the class used to select the correct type using the __typename defined in the GraphQL draft from Oct 2016
        /// </summary>
        public string Name { get; }
    }
}