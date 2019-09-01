using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.Introspection
{
    public class GraphQLIntrospectionTypeRef : GraphQLIntrospectionType
    {
        [GraphQLMaxDepth(8)]
        public GraphQLIntrospectionTypeRef OfType { get; set; }
    }
}
