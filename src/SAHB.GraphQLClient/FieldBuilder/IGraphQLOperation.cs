using SAHB.GraphQLClient.FieldBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAHB.GraphQL.Client.FieldBuilder
{
    /// <summary>
    /// Contains a GraphQL operation which should be executed
    /// </summary>
    public interface IGraphQLOperation
    {
        GraphQLOperationType OperationType { get; }
        IEnumerable<GraphQLField> SelectionSet { get; }
    }
}
