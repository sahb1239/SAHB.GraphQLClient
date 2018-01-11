using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Internal
{
    internal class Helper
    {
        internal static IEnumerable<GraphQLFieldArguments> GetAllArgumentsFromFields(IEnumerable<GraphQLField> fields)
        {
            return fields?.SelectMany(field => field.Arguments.Concat(GetAllArgumentsFromFields(field.Fields))) ?? Enumerable.Empty<GraphQLFieldArguments>();
        }
    }
}
