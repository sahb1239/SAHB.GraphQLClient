using System.Collections.Generic;

namespace SAHB.GraphQLClient.Introspection
{
    public class GraphQLIntrospectionField
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<GraphQLIntrospectionInputValue> Args { get; set; }
        public GraphQLIntrospectionTypeRef Type { get; set; }
        public bool IsDeprecated { get; set; }
        public string DeprecationReason { get; set; }
    }
}
