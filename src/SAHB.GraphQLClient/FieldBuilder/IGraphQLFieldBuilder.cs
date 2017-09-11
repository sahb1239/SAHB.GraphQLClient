using System;
using System.Collections.Generic;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
    public interface IGraphQLFieldBuilder
    {
        IEnumerable<GraphQLField> GetFields(Type type);
    }
}
