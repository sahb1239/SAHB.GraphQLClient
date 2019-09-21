using System.Collections.Generic;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.Introspection
{
    public class GraphQLIntrospectionFullType : GraphQLIntrospectionType
    {
        public string Description { get; set; }

        [GraphQLArguments("includeDeprecated", "Boolean", "fieldsIncludeDeprecated")]
        public IEnumerable<GraphQLIntrospectionField> Fields { get; set; }
        public IEnumerable<GraphQLIntrospectionInputValue> InputFields { get; set; }
        public IEnumerable<GraphQLIntrospectionTypeRef> Interfaces { get; set; }

        [GraphQLArguments("includeDeprecated", "Boolean", "enumValuesIncludeDeprecated")]
        public IEnumerable<GraphQLIntrospectionEnumValue> EnumValues { get; set; }

        public IEnumerable<GraphQLIntrospectionTypeRef> PossibleTypes { get; set; }
    }
}
