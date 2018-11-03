using System;
using System.Collections.Generic;
using SAHB.GraphQL.Client.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Tests.Mocks
{
    public class FieldBuilderMock : IGraphQLFieldBuilder
    {
        private readonly IEnumerable<GraphQLField> _fields;

        public FieldBuilderMock(IEnumerable<GraphQLField> fields)
        {
            _fields = fields;
        }

        public IGraphQLOperation GenerateOperation(Type type, GraphQLOperationType operationType)
        {
            return new GraphQLOperation(operationType, _fields);
        }
    }
}