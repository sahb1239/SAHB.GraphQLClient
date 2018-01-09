using System;

namespace SAHB.GraphQLClient.Builder
{
    // ReSharper disable once InconsistentNaming
    public interface IGraphQLBuilder
    {
        IGraphQLBuilder Field(string field, Action<IGraphQLQueryFieldBuilder> generator);
    }
}