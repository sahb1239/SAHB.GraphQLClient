using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    // ReSharper disable once InconsistentNaming
    public class GraphQLFieldNameAttribute : Attribute
    {
        public GraphQLFieldNameAttribute(string fieldName)
        {
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        }

        public string FieldName { get; }
    }
}