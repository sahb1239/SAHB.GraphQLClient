using System;
using System.Collections.Generic;
using SAHB.GraphQL.Client.FieldBuilder;

namespace SAHB.GraphQLClient.FieldBuilder
{
    public class GraphQLOperation : IGraphQLOperation
    {
        public GraphQLOperation(GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet)
        {
            OperationType = operationType;
            SelectionSet = selectionSet ?? throw new ArgumentNullException(nameof(selectionSet));
        }

        public GraphQLOperationType OperationType { get; }

        public IEnumerable<GraphQLField> SelectionSet { get; }
    }
}