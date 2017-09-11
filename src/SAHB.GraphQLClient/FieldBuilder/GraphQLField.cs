using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
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

        public string Alias { get; }
        public string Field { get; }
        public IEnumerable<GraphQLField> Fields { get; }
        public IEnumerable<GraphQLFieldArguments> Arguments { get; }

        internal Type Type { get; }
        internal PropertyInfo PropertyInfo { get; }
    }
}