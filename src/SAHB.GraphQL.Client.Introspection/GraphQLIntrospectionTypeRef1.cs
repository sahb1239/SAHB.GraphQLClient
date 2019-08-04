namespace SAHB.GraphQLClient.Introspection
{
    public class GraphQLIntrospectionTypeRef<T> : GraphQLIntrospectionType
    {
        public GraphQLTypeKind Kind { get; set; }
        public T OfType { get; set; }
    }
}
