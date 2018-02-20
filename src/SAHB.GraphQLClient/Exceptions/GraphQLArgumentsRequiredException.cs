using System.Collections.Generic;
using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Exceptions
{
    // ReSharper disable once InconsistentNaming
    /// <summary> 
    /// <see cref="GraphQLArgumentsRequiredException"/> is thrown when a required argument is not supplied for generating the query
    /// </summary> 
    public class GraphQLArgumentsRequiredException : GraphQLException
    {
        public GraphQLArgumentsRequiredException(IEnumerable<GraphQLFieldArguments> arguments) : base("The arguments:\n" + string.Join(", ", arguments.Select(argument => argument.VariableName + ":" + argument.ArgumentName)) + " is required")
        {
            Arguments = arguments;
        }

        public IEnumerable<GraphQLFieldArguments> Arguments { get; }
    }
}
