using System;

namespace SAHB.GraphQLClient.FieldBuilder.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class GraphQLMaxDepthAttribute : Attribute
    {
        public GraphQLMaxDepthAttribute(int maxDepth)
        {
            MaxDepth = maxDepth;
        }

        public int MaxDepth { get; set; }
    }
}
