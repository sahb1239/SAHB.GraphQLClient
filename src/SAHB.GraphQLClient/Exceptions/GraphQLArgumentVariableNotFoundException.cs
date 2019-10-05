using System;
using System.Collections.Generic;
using System.Linq;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Exceptions
{
    /// <summary>
    /// <see cref="GraphQLArgumentVariableNotFoundException"/> is thrown when a argument given to a <see cref="GraphQLQueryGeneratorFromFields"/> was not found in the fields and could therefore not be inserted
    /// </summary>
    public class GraphQLArgumentVariableNotFoundException : GraphQLException
    {
        public IEnumerable<GraphQLQueryArgument> Arguments { get; }

        public GraphQLArgumentVariableNotFoundException(IEnumerable<GraphQLQueryArgument> arguments) : base($"The arguments with the following variables could not be found:{Environment.NewLine}{string.Join(Environment.NewLine, arguments.Select(e => e.VariableName))}")
        {
            Arguments = arguments;
        }
    }
}