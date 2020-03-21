using GraphQL.Types;

namespace SAHB.GraphQL.Client.Testserver.Tests.Schemas.Hello
{
    public class HelloQuerySchema : Schema
    {
        public HelloQuerySchema()
        {
            Query = new GraphQLQuery();
        }

        private class GraphQLQuery : ObjectGraphType
        {
            public GraphQLQuery()
            {
                Field<StringGraphType>("hello", resolve: context => "query");
            }
        }
    }
}
