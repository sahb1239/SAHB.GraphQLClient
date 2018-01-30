using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Exceptions
{
    public class GraphQLArgumentsRequiredException : GraphQLException
    {
        public GraphQLArgumentsRequiredException(IEnumerable<GraphQLFieldArguments> arguments) : base("The arguments:\n" + string.Join(", ", arguments.Select(argument => argument.VariableName + ":" + argument.ArgumentName)) + " is required")
        {
            Arguments = arguments;
        }

        public IEnumerable<GraphQLFieldArguments> Arguments { get; }
    }
}
