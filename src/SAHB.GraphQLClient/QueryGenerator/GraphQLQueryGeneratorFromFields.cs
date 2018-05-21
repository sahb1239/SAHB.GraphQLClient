using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Internal;

namespace SAHB.GraphQLClient.QueryGenerator
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    public class GraphQLQueryGeneratorFromFields : IGraphQLQueryGeneratorFromFields
    {
        /// <inheritdoc />
        public string GetQuery(IEnumerable<GraphQLField> fields, params GraphQLQueryArgument[] arguments)
        {
            return GetQuery("query", fields.ToList(), arguments);
        }

        /// <inheritdoc />
        public string GetMutation(IEnumerable<GraphQLField> fields, params GraphQLQueryArgument[] arguments)
        {
            return GetQuery("mutation", fields.ToList(), arguments);
        }

        private string GetQuery(string queryType, ICollection<GraphQLField> fields, params GraphQLQueryArgument[] queryArguments)
        {
            // Get all the arguments from the fields
            var fieldArguments = Helper.GetAllArgumentsFromFields(fields).ToList();
            
            // Create mapping for each argument field
            IDictionary<GraphQLFieldArguments, GraphQLQueryArgument> arguments = new Dictionary<GraphQLFieldArguments, GraphQLQueryArgument>();
            ICollection<GraphQLFieldArguments> argumentsNotSet = new Collection<GraphQLFieldArguments>();
            List<string> duplicateVariableNames = new List<string>();
            foreach (var fieldArgument in fieldArguments)
            {
                // Find matching query arguments
                var queryArgument = queryArguments.Where(e => e.VariableName == fieldArgument.VariableName).Take(2).ToList();

                switch (queryArgument.Count)
                {
                    case 0:
                        // Set default value
                        if (fieldArgument.DefaultValue != null)
                        {
                            arguments.Add(fieldArgument, new GraphQLQueryArgument(fieldArgument.VariableName, fieldArgument.DefaultValue));
                            break;
                        }
                        // If no default was set we need to check if it was required
                        if (fieldArgument.IsRequired)
                        {
                            argumentsNotSet.Add(fieldArgument);
                        }
                        break;
                    case 1:
                        arguments.Add(fieldArgument, queryArgument[0]);
                        break;
                    default:
                        duplicateVariableNames.Add(queryArgument[0].VariableName);
                        break;
                }
            }

            // If any arguments was detected not set
            if (argumentsNotSet.Any())
            {
                throw new GraphQLArgumentsRequiredException(argumentsNotSet);
            }

            // If duplicate variable names has been detected
            if (duplicateVariableNames.Any())
            {
                throw new GraphQLDuplicateVariablesException(duplicateVariableNames);
            }

            // Get readonly arguments
            var readonlyArguments = new ReadOnlyDictionary<GraphQLFieldArguments, GraphQLQueryArgument>(arguments);

            // Get query
            var query = GetGraphQLQuery(queryType, GetArguments(readonlyArguments), GetFields(fields, readonlyArguments));
            var request = GetQueryRequest(query, readonlyArguments);

            // Logging
            if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"Generated the GraphQL query {request} from the fields:{Environment.NewLine + string.Join(Environment.NewLine, fields)}");
            }

            return request;
        }

        private bool ShouldInlineArgument(KeyValuePair<GraphQLFieldArguments, GraphQLQueryArgument> keyValuePair) =>
            ShouldInlineArgument(keyValuePair.Key, keyValuePair.Value);

        private bool ShouldInlineArgument(GraphQLFieldArguments fieldArgument, GraphQLQueryArgument queryArgument)
        {
            // If inline is forced we should just inline
            if (fieldArgument.InlineArgument.HasValue)
            {
                return fieldArgument.InlineArgument.Value;
            }

            // Else implicit inline if query argument is of specific types
            switch (queryArgument.ArgumentValue)
            {
                case string _:
                case bool _:
                case int _:
                case long _:
                case float _:
                case double _:
                    return true;
            }

            // Else don't inline
            return false;
        }

        private string GetArguments(IReadOnlyDictionary<GraphQLFieldArguments, GraphQLQueryArgument> arguments)
        {
            return string.Join(" ", arguments.Where(argument => !ShouldInlineArgument(argument)).Select(e => $"${e.Key.VariableName}:{e.Key.ArgumentType}"));
        }

        private string GetFields(IEnumerable<GraphQLField> fields, IReadOnlyDictionary<GraphQLFieldArguments, GraphQLQueryArgument> arguments)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");

            builder.Append(string.Join(" ", fields.Select(field =>
            {
                var fieldBuilder = new StringBuilder();

                // Append alias and field
                if (field.Alias == null || field.Alias.Equals(field.Field, StringComparison.OrdinalIgnoreCase))
                {
                    fieldBuilder.Append(field.Field);
                }
                else
                {
                    fieldBuilder.Append(field.Alias + ":" + field.Field);
                }

                // Append arguments
                // Format: (argumentName:$VariableName argumentName:$VariableName)
                var fieldArguments = field.Arguments?.ToDictionary(argument => argument,
                    argument => arguments.FirstOrDefault(e => e.Key == argument).Value).Where(e => e.Value != null);
                if (fieldArguments?.Any() ?? false)
                {
                    fieldBuilder.Append("(");

                    fieldBuilder.Append(string.Join(" ",
                        fieldArguments.Select(
                            argument => argument.Key.ArgumentName + ":" +
                                        (ShouldInlineArgument(argument)
                                            ? JsonConvert.SerializeObject(argument.Value.ArgumentValue)
                                            : "$" + argument.Key.VariableName))));

                    fieldBuilder.Append(")");
                }

                // Append subquery
                if (field.Fields?.Any() ?? false)
                {
                    fieldBuilder.Append(GetFields(field.Fields, arguments));
                }

                // Get other possible subTypes
                if (field.PossibleTypes?.Any() ?? false)
                {
                    foreach (var possibleType in field.PossibleTypes)
                    {
                        // Append subquery
                        fieldBuilder.Append(
                            $" ... on {possibleType.TypeName}{GetFields(possibleType.Fields, arguments)}");
                    }

                    // Add the __typename to the query to determine the type. __typename is defined in draft GraphQL draft Oct 2016 4.1.4
                    fieldBuilder.Append(" __typename");
                }

                return fieldBuilder.ToString();
            })));

            builder.Append("}");
            return builder.ToString();
        }

        private string GetGraphQLQuery(string queryType, string argumentVariableDeclaration, string fields)
        {
            // Get argument string
            if (!string.IsNullOrEmpty(argumentVariableDeclaration))
            {
                argumentVariableDeclaration = $"({argumentVariableDeclaration})";
            }

            // Return query
            return queryType + argumentVariableDeclaration + fields;
        }

        private string GetQueryRequest(string query, IReadOnlyDictionary<GraphQLFieldArguments, GraphQLQueryArgument> arguments)
        {
            var variables = arguments.Where(e => !ShouldInlineArgument(e)).ToArray();
            if (variables.Any())
            {
                return JsonConvert.SerializeObject(new { query, variables = variables.ToDictionary(e => e.Value.VariableName, e => e.Value.ArgumentValue) });
            }

            return JsonConvert.SerializeObject(new { query });
        }

        #region Logging

        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// Contains a logger factory for the GraphQLHttpClient
        /// </summary>
        public ILoggerFactory LoggerFactory
        {
            internal get { return _loggerFactory; }
            set
            {
                _loggerFactory = value;
                if (_loggerFactory != null)
                {
                    Logger = _loggerFactory.CreateLogger<GraphQLQueryGeneratorFromFields>();
                }
            }
        }

        /// <summary>
        /// Contains the logger for the class
        /// </summary>
        private ILogger<GraphQLQueryGeneratorFromFields> Logger { get; set; }

        #endregion
    }
}
