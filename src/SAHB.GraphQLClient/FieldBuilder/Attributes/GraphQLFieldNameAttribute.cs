using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Attribute which defines the current property or class should map to a specific GraphQL field name
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class GraphQLFieldNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a attribute which defines the current property or class should map to a specific GraphQL field name
        /// </summary>
        /// <param name="fieldName">The GraphQL field</param>
        public GraphQLFieldNameAttribute(string fieldName)
        {
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        }

        /// <summary>
        /// The GraphQL field
        /// </summary>
        public string FieldName { get; }
    }
}
