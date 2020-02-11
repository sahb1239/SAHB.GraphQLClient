using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Ignores the GraphQL field
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class GraphQLFieldIgnoreAttribute : Attribute
    {
    }
}
