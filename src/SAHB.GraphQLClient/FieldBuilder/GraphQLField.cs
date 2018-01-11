using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL field used to contain metadata which can be used for generating a GraphQL query
    /// </summary>
    public class GraphQLField
    {
        /// <summary>
        /// Initilizes a GraphQL field used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="alias">GraphQL alias</param>
        /// <param name="field">GraphQL field</param>
        /// <param name="fields">Subfields</param>
        /// <param name="arguments">Arguments for the current field</param>
        public GraphQLField(string alias, string field, IEnumerable<GraphQLField> fields,
            IEnumerable<GraphQLFieldArguments> arguments)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));

            Alias = alias;
            Fields = (fields ?? Enumerable.Empty<GraphQLField>()).ToList();
            Arguments = (arguments ?? Enumerable.Empty<GraphQLFieldArguments>()).ToList();
        }

        /// <summary>
        /// GraphQL alias
        /// </summary>
        public string Alias { get; internal set; }
        
        /// <summary>
        /// GraphQL field
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Subfields
        /// </summary>
        public ICollection<GraphQLField> Fields { get; }

        /// <summary>
        /// Arguments for the current field
        /// </summary>
        public ICollection<GraphQLFieldArguments> Arguments { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Field: {Field}");
            builder.AppendLine($"Alias: {(Alias ?? "null")}");
            if (Arguments.Any())
            {
                builder.AppendLine($"Arguments: {IndentAndAddStart(String.Join(Environment.NewLine, Arguments))}");
            }
            if (Fields.Any())
            {
                builder.AppendLine($"Fields: {IndentAndAddStart(String.Join(Environment.NewLine, Fields))}");
            }
            return builder.ToString();
        }

        private string IndentAndAddStart(string text)
        {
            return (Environment.NewLine + text).Replace(Environment.NewLine, Environment.NewLine + "   ");
        }
    }
}