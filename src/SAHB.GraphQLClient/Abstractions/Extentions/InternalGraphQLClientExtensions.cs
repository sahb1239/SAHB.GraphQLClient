using System;
using System.Collections.Generic;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.Builder.Internal;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient
{
    internal static class InternalGraphQLClientExtensions
    {
        public static IEnumerable<GraphQLField> GetSelectionSet<T>(this IGraphQLClient client)
        {
            return client.FieldBuilder.GenerateSelectionSet(typeof(T));
        }

        public static IEnumerable<GraphQLField> GetSelectionSet(this Action<IGraphQLBuilder> queryBuilder)
        {
            // Get builder
            var build = new GraphQLBuilder();
            queryBuilder(build);

            // Get the fields and query
            var selectionSet = build.GetFields();

            return selectionSet;
        }
    }
}
