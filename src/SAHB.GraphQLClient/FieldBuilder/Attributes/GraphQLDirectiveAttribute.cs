using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class GraphQLDirectiveAttribute : Attribute
    {
        /// <summary>
        /// Initializes a attribute which defines a directive argument which is used for a GraphQL field
        /// </summary>
        /// <param name="directiveName">The directive name used in the GraphQL query</param>
        public GraphQLDirectiveAttribute(string directiveName)
        {
            DirectiveName = directiveName ?? throw new ArgumentNullException(nameof(directiveName));
        }

        /// <summary>
        /// Name of the Directive
        /// </summary>
        public string DirectiveName { get; }
    }
}
