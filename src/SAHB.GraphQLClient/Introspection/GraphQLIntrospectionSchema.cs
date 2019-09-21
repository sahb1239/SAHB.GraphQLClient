using System.Collections.Generic;

namespace SAHB.GraphQLClient.Introspection
{
    public class GraphQLIntrospectionSchema
    {
        public GraphQLIntrospectionType QueryType { get; set; }
        public GraphQLIntrospectionType MutationType { get; set; }
        public GraphQLIntrospectionType SubscriptionType { get; set; }
        public IEnumerable<GraphQLIntrospectionFullType> Types { get; set; }
        public IEnumerable<GraphQLIntrospectionDirective> Directives { get; set; }
    }
}
