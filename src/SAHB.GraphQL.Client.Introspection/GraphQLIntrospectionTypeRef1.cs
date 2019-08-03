namespace SAHB.GraphQLClient.Introspection
{
    public class GraphQLIntrospectionTypeRef<T>
    {
        public GraphQLTypeKind Kind { get; set; }
        public string Name { get; set; }
        public T OfType { get; set; }
    }
}
