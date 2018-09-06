using System.Collections.Generic;
using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Internal
{
    internal class Helper
    {
        internal static IEnumerable<GraphQLFieldArguments> GetAllArgumentsFromFields(IEnumerable<GraphQLField> fields)
        {
            return fields?.SelectMany(field => field.Arguments.Concat(GetAllArgumentsFromFields(field.SelectionSet))) ?? Enumerable.Empty<GraphQLFieldArguments>();
        }
    }
}
