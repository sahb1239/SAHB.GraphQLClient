using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Internal
{
    internal class GraphQLQueryArgumentWithOverriddenVariable : GraphQLQueryArgument
    {
        public GraphQLQueryArgumentWithOverriddenVariable(GraphQLQueryArgument argument)
            : base(argument.VariableName, argument.Field, argument.ArgumentValue)
        {

        }
    }
}
