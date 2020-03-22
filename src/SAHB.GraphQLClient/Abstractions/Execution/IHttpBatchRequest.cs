using System;
using System.Collections.Generic;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Execution
{
    public interface IHttpBatchRequest
    {
        IReadOnlyCollection<GraphQLField> GetSelectionSet();
        ICollection<GraphQLQueryArgument> GetArguments();
        ICollection<GraphQLQueryArgument> GetDirectiveArguments();
        Func<GraphQLField, bool> GetQueryFilter();
        IEnumerable<ValidationError> Validate(GraphQLIntrospectionSchema graphQLIntrospectionSchema);
    }
}
