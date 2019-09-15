using GraphQL.Types;

namespace SAHB.GraphQL.Client.Testserver.Tests.Schemas.HelloArgument
{
    public class HelloArgumentQuerySchema : Schema
    {
        public HelloArgumentQuerySchema()
        {
            Query = new GraphQLQuery();
        }

        private class GraphQLQuery : ObjectGraphType
        {
            public GraphQLQuery()
            {
                Field<StringGraphType>(
                    "hello", 
                    resolve: context => "query", 
                    arguments: new QueryArguments(
                        new QueryArgument<StringGraphType>
                        {
                            Name = "argument1"
                        },
                        new QueryArgument<IntGraphType>
                        {
                            Name = "argument2"
                        },
                        new QueryArgument<IdGraphType>
                        {
                            Name = "argument3"
                        }
                    )
                );
            }
        }
    }
}
