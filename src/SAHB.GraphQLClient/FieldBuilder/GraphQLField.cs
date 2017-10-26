using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL field used to contain metadata which can be used for generating a GraphQL query
    /// </summary>
    public class GraphQLField
    {
        public GraphQLField(string alias, string field, IEnumerable<GraphQLField> fields,
            IEnumerable<GraphQLFieldArguments> arguments, Type type, PropertyInfo propertyInfo)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));

            Alias = alias;
            Fields = fields ?? Enumerable.Empty<GraphQLField>();
            Arguments = arguments ?? Enumerable.Empty<GraphQLFieldArguments>();
            Type = type;
            PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// GraphQL alias
        /// </summary>
        public string Alias { get; }
        
        /// <summary>
        /// GraphQL field
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Subfields
        /// </summary>
        public IEnumerable<GraphQLField> Fields { get; }

        /// <summary>
        /// Arguments for the current field
        /// </summary>
        public IEnumerable<GraphQLFieldArguments> Arguments { get; }

        internal Type Type { get; }
        internal PropertyInfo PropertyInfo { get; }
    }
}