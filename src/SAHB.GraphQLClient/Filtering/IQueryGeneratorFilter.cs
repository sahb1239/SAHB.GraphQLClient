using System;
using System.Linq.Expressions;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQL.Client.Filtering
{
    public interface IQueryGeneratorFilter
    {
        Func<GraphQLField, bool> GetFilter<T>(Expression<Func<T, object>> expression);
    }
}
