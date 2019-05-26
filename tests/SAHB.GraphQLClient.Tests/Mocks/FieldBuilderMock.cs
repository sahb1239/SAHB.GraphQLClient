using System;
using System.Collections.Generic;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Tests.Mocks
{
    public class FieldBuilderMock : IGraphQLFieldBuilder
    {
        private readonly IEnumerable<IGraphQLField> _fields;

        public FieldBuilderMock(IEnumerable<IGraphQLField> fields)
        {
            _fields = fields;
        }

        public IEnumerable<IGraphQLField> GenerateSelectionSet(Type type)
        {
            return _fields;
        }

        public IEnumerable<IGraphQLField> GetFields(Type type) => GenerateSelectionSet(type);
    }
}