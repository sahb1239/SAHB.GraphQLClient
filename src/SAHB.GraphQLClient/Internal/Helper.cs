using System.Collections.Generic;
using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Internal
{
    internal class Helper
    {
        internal static IDictionary<string, IEnumerable<GraphQLFieldArguments>> GetAllArgumentsFromFields(IEnumerable<GraphQLField> fields)
        {
            var dictionary = new Dictionary<string, IEnumerable<GraphQLFieldArguments>>();
            GetAllArgumentsFromFields(fields, dictionary, null);
            return dictionary;
        }

        private static void GetAllArgumentsFromFields(IEnumerable<GraphQLField> fields, Dictionary<string, IEnumerable<GraphQLFieldArguments>> dictionary, string path)
        {
            foreach (var field in fields)
            {
                var currentPathPart = field.Alias ?? field.Field;
                var fieldPath = path == null ? 
                    currentPathPart : 
                    string.Join(".", path, currentPathPart);

                // Add all arguments
                if (field.Arguments.Any())
                {
                    dictionary.Add(fieldPath, field.Arguments);
                }

                // Add all arguments from selectionSet (added by providing same dictionary)
                GetAllArgumentsFromFields(field.SelectionSet, dictionary, fieldPath);
            }
        }
    }
}
