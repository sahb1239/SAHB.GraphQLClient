using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    // ReSharper disable once InconsistentNaming
    public class GraphQLFieldIgnoreAttribute : Attribute
    {
    }
}