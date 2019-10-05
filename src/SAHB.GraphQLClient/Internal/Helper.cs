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

                // Get arguments
                List<GraphQLFieldArguments> arguments = new List<GraphQLFieldArguments>();

                // Add all arguments
                if (field?.Arguments.Any() ?? false)
                {
                    arguments.AddRange(field.Arguments);
                }

                // Add all directive arguments
                if (field?.Directives.Any() ?? false)
                {
                    foreach (var directive in field.Directives)
                    {
                        if (directive.Arguments?.Any() ?? false)
                        {
                            arguments.AddRange(directive.Arguments);
                        }
                    }
                }

                if (arguments.Any())
                {
                    dictionary.Add(fieldPath, arguments);
                }

                // Add all arguments from selectionSet (added by providing same dictionary)
                GetAllArgumentsFromFields(field.SelectionSet, dictionary, fieldPath);
            }
        }
    }
}
