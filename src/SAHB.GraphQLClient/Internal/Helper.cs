using System.Collections.Generic;
using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Internal
{
    internal class Helper
    {
        internal static IEnumerable<IGraphQLArguments> GetAllArgumentsFromFields(IEnumerable<IGraphQLField> fields)
        {
            return fields?.SelectMany(field => field.Arguments.Concat(GetAllArgumentsFromFields(field.SelectionSet))) ?? Enumerable.Empty<IGraphQLArguments>();
        }
    }
}
