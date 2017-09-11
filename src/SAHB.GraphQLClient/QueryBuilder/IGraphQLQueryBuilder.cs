using System.Collections.Generic;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.QueryBuilder
{
    // ReSharper disable once InconsistentNaming
    public interface IGraphQLQueryBuilder
    {
        string GetQuery<T>(params GraphQLQueryArgument[] arguments);
        string GetMutation<T>(params GraphQLQueryArgument[] arguments);
        string GetQuery(IEnumerable<GraphQLField> fields, params GraphQLQueryArgument[] arguments);
        string GetMutation(IEnumerable<GraphQLField> fields, params GraphQLQueryArgument[] arguments);
    }
}