using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SAHB.GraphQLClient.Introspection
{
    // http://facebook.github.io/graphql/October2016/#sec-Type-Kinds
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GraphQLTypeKind
    {
        [EnumMember(Value = "SCALAR")]
        Scalar,
        [EnumMember(Value = "OBJECT")]
        Object,
        [EnumMember(Value = "UNION")]
        Union,
        [EnumMember(Value = "INTERFACE")]
        Interface,
        [EnumMember(Value = "ENUM")]
        Enum,
        [EnumMember(Value = "INPUT_OBJECT")]
        InputObject,
        [EnumMember(Value = "LIST")]
        List,
        [EnumMember(Value = "NON_NULL")]
        NonNull
    }
}
