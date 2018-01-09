namespace SAHB.GraphQLClient.Builder
{
    // ReSharper disable once InconsistentNaming
    public interface IGraphQLQueryFieldBuilder : IGraphQLBuilder
    {
        IGraphQLQueryFieldBuilder Alias(string alias);
        IGraphQLQueryFieldBuilder Argument(string argumentName, string argumentType, string variableName);
    }
}