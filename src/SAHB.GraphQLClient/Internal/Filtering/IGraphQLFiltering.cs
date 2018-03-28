using System.Linq.Expressions;

namespace SAHB.GraphQLClient.Internal.Filtering
{
    internal interface IGraphQLFiltering
    {
        string GetFilterArgument(Expression expression);
    }
}
