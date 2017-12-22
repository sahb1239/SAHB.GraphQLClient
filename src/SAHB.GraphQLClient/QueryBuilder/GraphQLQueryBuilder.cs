using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.QueryBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    public class GraphQLQueryBuilder : IGraphQLQueryBuilder, IGraphQLQueryBuilderFromFields
    {
        private readonly IGraphQLFieldBuilder _graphQlFieldBuilder;

        public GraphQLQueryBuilder(IGraphQLFieldBuilder graphQlFieldBuilder)
        {
            _graphQlFieldBuilder = graphQlFieldBuilder;
        }

        /// <inheritdoc />
        public string GetQuery(Type type, params GraphQLQueryArgument[] arguments)
        {
            return GetQuery(_graphQlFieldBuilder.GetFields(type), arguments);
        }

        /// <inheritdoc />
        public string GetMutation(Type type, params GraphQLQueryArgument[] arguments)
        {
            return GetMutation(_graphQlFieldBuilder.GetFields(type), arguments);
        }

        public string GetQuery(IEnumerable<GraphQLField> fields, params GraphQLQueryArgument[] arguments)
        {
            return GetQuery("query", fields, arguments);
        }

        public string GetMutation(IEnumerable<GraphQLField> fields, params GraphQLQueryArgument[] arguments)
        {
            return GetQuery("mutation", fields, arguments);
        }

        private string GetQuery(string queryType, IEnumerable<GraphQLField> fields, params GraphQLQueryArgument[] arguments)
        {
            var query = GetGraphQLQuery(queryType, arguments, GetFields(fields));
            var request = GetQueryRequest(query, arguments);

            return request;
        }

        private string GetFields(IEnumerable<GraphQLField> fields)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");

            builder.Append(string.Join(" ", fields.Select(field =>
            {
                var fieldBuilder = new StringBuilder();

                // Append alias and field
                if (field.Alias == field.Field)
                {
                    fieldBuilder.Append(field.Field);
                }
                else
                {
                    fieldBuilder.Append(field.Alias + ":" + field.Field);
                }

                // Append arguments
                // Format: (argumentName:$VariableName argumentName:$VariableName)
                if (field.Arguments?.Any() ?? false)
                {
                    fieldBuilder.Append("(");

                    fieldBuilder.Append(string.Join(" ",
                        field.Arguments.Select(
                            argument => argument.ArgumentName + ":" + "$" + argument.VariableName)));

                    fieldBuilder.Append(")");
                }

                // Append subquery
                if (field.Fields?.Any() ?? false)
                {
                    fieldBuilder.Append(GetFields(field.Fields));
                }

                return fieldBuilder.ToString();
            })));

            builder.Append("}");
            return builder.ToString();
        }

        private string GetGraphQLQuery(string queryType, IEnumerable<GraphQLQueryArgument> arguments, string fields)
        {
            // Get argument string
            var argument = string.Join(" ", arguments.Select(e => "$" + e.VariableName + ":" + e.ArgumentType));
            if (!string.IsNullOrEmpty(argument))
            {
                argument = $"({argument})";
            }

            // Return query
            return queryType + argument + fields;
        }

        private string GetQueryRequest(string query, params GraphQLQueryArgument[] arguments)
        {
            if (arguments.Any())
            {
                return JsonConvert.SerializeObject(new { query = query, Variables = JsonConvert.SerializeObject(arguments.ToDictionary(e => e.VariableName, e => e.ArgumentValue)) });
            }

            return JsonConvert.SerializeObject(new { query = query });
        }
    }
}
