using System.Linq.Expressions;

namespace SAHB.GraphQLClient.Internal.Filtering
{
    internal interface IGraphQLFiltering
    {
        object GetFilterArgument(Expression expression);
    }
}
