namespace SAHB.GraphQLClient.Introspection
{
    public class GraphQLIntrospectionInputValue
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GraphQLIntrospectionTypeRef Type { get; set; }
        public string DefaultValue { get; set; }
    }
}
