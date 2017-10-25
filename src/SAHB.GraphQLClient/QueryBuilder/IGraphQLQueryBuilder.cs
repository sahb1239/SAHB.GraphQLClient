namespace SAHB.GraphQLClient.QueryBuilder
{
    // ReSharper disable once InconsistentNaming
    public interface IGraphQLQueryBuilder
    {
        string GetQuery<T>(params GraphQLQueryArgument[] arguments);
        string GetMutation<T>(params GraphQLQueryArgument[] arguments);
    }
}