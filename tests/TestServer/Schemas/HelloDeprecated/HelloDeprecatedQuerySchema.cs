using GraphQL.Types;

namespace SAHB.GraphQL.Client.Testserver.Tests.Schemas.HelloDeprecated
{
    public class HelloDeprecatedQuerySchema : Schema
    {
        public HelloDeprecatedQuerySchema()
        {
            Query = new GraphQLQuery();
        }

        private class GraphQLQuery : ObjectGraphType
        {
            public GraphQLQuery()
            {
                Field<StringGraphType>("hello", resolve: context => "query", deprecationReason: "It's deprecated");
            }
        }
    }
}
