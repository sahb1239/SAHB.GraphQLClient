using System.Collections.Generic;

namespace SAHB.GraphQLClient.Introspection
{
    public class GraphQLIntrospectionDirective
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<GraphQLIntrospectionInputValue> Args { get; set; }
        public IEnumerable<GraphQLIntrospectionDirectiveLocation> Locations { get; set; }
    }
}
