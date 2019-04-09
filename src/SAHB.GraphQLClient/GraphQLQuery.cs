using System;
using System.Collections.Generic;
using SAHB.GraphQL.Client;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient
{
    public class GraphQLQueryToExecute : IGraphQLQueryToExecute
    {
        public GraphQLQueryToExecute(GraphQLOperationType operationType, IEnumerable<IGraphQLField> selectionSet, IEnumerable<GraphQLQueryArgument> arguments)
        {
            OperationType = operationType;
            SelectionSet = selectionSet ?? throw new ArgumentNullException(nameof(selectionSet));
            Arguments = arguments;
        }

        public IEnumerable<GraphQLQueryArgument> Arguments { get; }

        public GraphQLOperationType OperationType { get; }

        public IEnumerable<IGraphQLField> SelectionSet { get; }
    }
}