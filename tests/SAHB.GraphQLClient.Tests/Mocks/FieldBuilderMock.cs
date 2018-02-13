using System;
using System.Collections.Generic;
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

        public IEnumerable<GraphQLField> GetFields(Type type)
        {
            return _fields;
        }
    }
}