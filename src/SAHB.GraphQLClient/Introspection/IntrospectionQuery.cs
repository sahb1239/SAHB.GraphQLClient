using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.Introspection
{
    // https://github.com/graphql/graphql-js/blob/f59f44a06ab4d433df4b056d150f79885b62243f/src/utilities/introspectionQuery.js
    // https://github.com/graphql/graphiql/blob/master/src/utility/introspectionQueries.js
    public class GraphQLIntrospectionQuery
    {
        [GraphQLFieldName("__schema")]
        public GraphQLIntrospectionSchema Schema { get; set; }
    }
}
