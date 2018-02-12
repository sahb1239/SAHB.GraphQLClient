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

        private string GetQuery(string queryType, ICollection<GraphQLField> fields, params GraphQLQueryArgument[] arguments)
        {
            // Get all the arguments from the fields
            var fieldArguments = Helper.GetAllArgumentsFromFields(fields).ToList();
            
            // Validate that each argument is set
            ICollection<GraphQLFieldArguments> argumentsNotSet = new Collection<GraphQLFieldArguments>();
            foreach (var argument in fieldArguments)
            {
                // Validate if the argument is required and not specified in the arguments recieved in parameters
                if (argument.IsRequired && arguments.All(e => e.VariableName != argument.VariableName))
                {
                    argumentsNotSet.Add(argument);
                }
            }

            // If any arguments was detected not set
            if (argumentsNotSet.Any())
            {
                throw new GraphQLArgumentsRequiredException(argumentsNotSet);
            }

            // Define filterArguments function
            bool FilterArguments(GraphQLFieldArguments argument) => arguments.Any(e => e.VariableName == argument.VariableName);

            // Get query
            var query = GetGraphQLQuery(queryType, GetArguments(fieldArguments, FilterArguments), GetFields(fields, argument => arguments.Any(e => e.VariableName == argument.VariableName)));
            var request = GetQueryRequest(query, arguments);

            // Logging
            if (Logger != null && Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation($"Generated the GraphQL query {request} from the fields:{Environment.NewLine + string.Join(Environment.NewLine, fields)}");
            }

            return request;
        }

        private string GetArguments(IEnumerable<GraphQLFieldArguments> argumentsFromFields, Func<GraphQLFieldArguments, bool> filterArguments)
        {
            return string.Join(" ", argumentsFromFields.Where(filterArguments).Select(e => $"${e.VariableName}:{e.ArgumentType}"));
        }

        private string GetFields(IEnumerable<GraphQLField> fields, Func<GraphQLFieldArguments, bool> filterArguments)
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
                var fieldArguments = filterArguments == null ? field.Arguments : field.Arguments?.Where(filterArguments).ToList();
                if (fieldArguments?.Any() ?? false)
                {
                    fieldBuilder.Append("(");

                    fieldBuilder.Append(string.Join(" ",
                        fieldArguments.Select(
                            argument => argument.ArgumentName + ":" + "$" + argument.VariableName)));

                    fieldBuilder.Append(")");
                }

                // Append subquery
                if (field.Fields?.Any() ?? false)
                {
                    fieldBuilder.Append(GetFields(field.Fields, filterArguments));
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

        private string GetQueryRequest(string query, GraphQLQueryArgument[] arguments)
        {
            if (arguments.Any())
            {
                return JsonConvert.SerializeObject(new { query = query, variables = (arguments.ToDictionary(e => e.VariableName, e => e.ArgumentValue)) });
            }

            return JsonConvert.SerializeObject(new { query = query });
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
