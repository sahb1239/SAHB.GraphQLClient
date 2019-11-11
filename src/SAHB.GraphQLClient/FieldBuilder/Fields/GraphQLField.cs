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
        /// Initializes a GraphQL field used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="alias">GraphQL alias</param>
        /// <param name="field">GraphQL field</param>
        /// <param name="fields">Subfields</param>
        /// <param name="arguments">Arguments for the current field</param>
        public GraphQLField(string alias, string field, IEnumerable<GraphQLField> fields,
            IEnumerable<GraphQLFieldArguments> arguments) : this(alias, field, fields, arguments, null, null)
        {
        }

        /// <summary>
        /// Initilizes a GraphQL field used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="alias">GraphQL alias</param>
        /// <param name="field">GraphQL field</param>
        /// <param name="fields">Subfields</param>
        /// <param name="arguments">Arguments for the current field</param>
        /// <param name="type">Default deserilzation type which should be deserilized to if no match is found in <paramref name="targetTypes"/></param>
        /// <param name="targetTypes">The types which should be deserilized to based on the __typename GraphQL field</param>
        public GraphQLField(string alias, string field, IEnumerable<GraphQLField> fields,
            IEnumerable<GraphQLFieldArguments> arguments, Type type, IDictionary<string, GraphQLTargetType> targetTypes)
            : this(alias, field, fields, arguments, null, type, targetTypes)
        {
        }

        /// <summary>
        /// Initilizes a GraphQL field used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="alias">GraphQL alias</param>
        /// <param name="field">GraphQL field</param>
        /// <param name="fields">Subfields</param>
        /// <param name="arguments">Arguments for the current field</param>
        /// <param name="directives">Directives for the current field</param>
        /// <param name="type">Default deserilzation type which should be deserilized to if no match is found in <paramref name="targetTypes"/></param>
        /// <param name="targetTypes">The types which should be deserilized to based on the __typename GraphQL field</param>
        public GraphQLField(string alias, string field, IEnumerable<GraphQLField> fields,
            IEnumerable<GraphQLFieldArguments> arguments, IEnumerable<GraphQLFieldDirective> directives,
            Type type, IDictionary<string, GraphQLTargetType> targetTypes)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));

            Alias = alias;
            SelectionSet = (fields ?? Enumerable.Empty<GraphQLField>()).ToList();
            Arguments = (arguments ?? Enumerable.Empty<GraphQLFieldArguments>()).ToList();
            Directives = (directives ?? Enumerable.Empty<GraphQLFieldDirective>()).ToList();

            BaseType = type;
            TargetTypes = (targetTypes ?? new Dictionary<string, GraphQLTargetType>());

            UpdateParentFieldForSelectionSet();
        }

        private void UpdateParentFieldForSelectionSet()
        {
            // Set parent for fields
            foreach (var selectionField in SelectionSet)
            {
                if (selectionField.ParentPath != null && selectionField.ParentPath != this.Path)
                {
                    throw new ArgumentException($"Field {selectionField.Field} with alias {selectionField.Alias} already has a parent set");
                }

                selectionField.ParentPath = this.Path;
            }
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
        /// The selection set for the field
        /// </summary>
        public ICollection<GraphQLField> SelectionSet { get; }

        /// <summary>
        /// Parent field
        /// </summary>
        public string ParentPath { get; private set; }

        /// <summary>
        /// Get the path of the field
        /// </summary>
        public string Path =>
            ParentPath == null ?
                Alias :
                $"{ParentPath}.{Alias}";

        /// <summary>
        /// Arguments for the current field
        /// </summary>
        public ICollection<GraphQLFieldArguments> Arguments { get; }

        /// <summary>
        /// Returns the type which should be deserilized to based on the __typename field
        /// </summary>
        public IDictionary<string, GraphQLTargetType> TargetTypes { get; }

        /// <summary>
        /// Returns the deserilzation type which should be deserilized to if no match is found in <see cref="TargetTypes"/>
        /// </summary>
        public Type BaseType { get; }

        /// <summary>
        /// The directives which should be applied to the field
        /// </summary>
        public ICollection<GraphQLFieldDirective> Directives { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Field: {Field}");
            builder.AppendLine($"Alias: {(Alias ?? "null")}");
            if (BaseType != null)
            {
                builder.AppendLine($"Base type: {BaseType.FullName}");
            }
            if (Arguments.Any())
            {
                builder.AppendLine($"Arguments: {IndentAndAddStart(String.Join(Environment.NewLine, Arguments))}");
            }
            if (Directives.Any())
            {
                builder.AppendLine($"Directives: {IndentAndAddStart(String.Join(Environment.NewLine, Directives))}");
            }
            if (SelectionSet.Any())
            {
                builder.AppendLine($"Fields: {IndentAndAddStart(String.Join(Environment.NewLine, SelectionSet))}");
            }
            return builder.ToString();
        }

        private string IndentAndAddStart(string text)
        {
            return (Environment.NewLine + text).Replace(Environment.NewLine, Environment.NewLine + "   ");
        }
    }
}
