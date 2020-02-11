using System;
using System.Collections.Generic;
using System.Linq;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Internal
{
    internal class Helper
    {
        internal static IEnumerable<GraphQLField> GetFilteredSelectionSet(IEnumerable<GraphQLField> selectionSet, Func<GraphQLField, bool> filter)
        {
            if (filter == null)
                return selectionSet;

            var filteredSelectionSet = selectionSet.Where(filter);
            return filteredSelectionSet;
        }

        internal static IDictionary<string, IEnumerable<GraphQLFieldArguments>> GetAllArgumentsFromFields(IEnumerable<GraphQLField> selectionSet, Func<GraphQLField, bool> filter)
        {
            var dictionary = new Dictionary<string, IEnumerable<GraphQLFieldArguments>>();
            GetAllArgumentsFromFields(selectionSet, filter, dictionary, null);
            return dictionary;
        }

        private static void GetAllArgumentsFromFields(IEnumerable<GraphQLField> selectionSet, Func<GraphQLField, bool> filter, Dictionary<string, IEnumerable<GraphQLFieldArguments>> dictionary, string path)
        {
            foreach (var field in GetFilteredSelectionSet(selectionSet, filter))
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
                GetAllArgumentsFromFields(field.SelectionSet, null, dictionary, fieldPath);
            }
        }
    }
}
