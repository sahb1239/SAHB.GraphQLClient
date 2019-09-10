using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQL.Client.Internal
{
    internal class GraphQLQueryArgumentWithOverriddenVariable : GraphQLQueryArgument
    {
        public GraphQLQueryArgumentWithOverriddenVariable(GraphQLQueryArgument argument)
            : base(argument.VariableName, argument.Field, argument.ArgumentValue)
        {

        }
    }
}
