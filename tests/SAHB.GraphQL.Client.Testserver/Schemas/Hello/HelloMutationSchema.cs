using GraphQL.Types;

namespace SAHB.GraphQL.Client.Testserver.Tests.Schemas.Hello
{
    public class HelloMutationSchema : Schema
    {
        public HelloMutationSchema()
        {
            Mutation = new GraphQLMutation();
        }

        private class GraphQLMutation : ObjectGraphType
        {
            public GraphQLMutation()
            {
                Field<StringGraphType>("hello", resolve: context => "mutation");
            }
        }
    }
}
