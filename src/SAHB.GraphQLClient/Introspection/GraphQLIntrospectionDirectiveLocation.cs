using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SAHB.GraphQLClient.Introspection
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GraphQLIntrospectionDirectiveLocation
    {
        [EnumMember(Value = "QUERY")]
        Query,
        [EnumMember(Value = "MUTATION")]
        Mutation,
        [EnumMember(Value = "ENUM")]
        Enum,
        [EnumMember(Value = "ENUM_VALUE")]
        EnumValue,
        [EnumMember(Value = "FIELD")]
        Field,
        [EnumMember(Value = "FIELD_DEFINITION")]
        FieldDefinition,
        [EnumMember(Value = "FRAGMENT_DEFINITION")]
        FragmentDefinition,
        [EnumMember(Value = "FRAGMENT_SPREAD")]
        FragmentSpread,
        [EnumMember(Value = "INLINE_FRAGMENT")]
        InlineFragment
    }
}
