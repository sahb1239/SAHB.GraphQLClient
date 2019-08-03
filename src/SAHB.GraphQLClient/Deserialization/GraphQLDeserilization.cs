using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAHB.GraphQLClient.Internal;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Result;

namespace SAHB.GraphQLClient.Deserialization
{
    /// <summary>
    /// Default implementation of the <see cref="IGraphQLDeserialization"/>
    /// </summary>
    public class GraphQLDeserilization : IGraphQLDeserialization
    {
        /// <inheritdoc />
        public GraphQLDataResult<T> DeserializeResult<T>(string graphQLResult, IEnumerable<GraphQLField> fields) where T : class
        {
            // Get all fieldConverters
            var converters = GetFieldConverters(fields);

            // Get JsonSerilizerSettings
            var settings = new JsonSerializerSettings();
            foreach (var converter in converters)
            {
                settings.Converters.Add(converter);
            }

            // Deserilize
            return JsonConvert.DeserializeObject<GraphQLDataResult<T>>(graphQLResult, settings);
        }

        /// <inheritdoc />
        public T DeserializeResult<T>(JObject jsonObject, IEnumerable<GraphQLField> fields) where T : class
        {
            // Get all fieldConverters
            var converters = GetFieldConverters(fields);

            // Get JsonSerilizerSettings
            var settings = new JsonSerializer();
            foreach (var converter in converters)
            {
                settings.Converters.Add(converter);
            }

            return jsonObject.ToObject<T>(settings);
        }

        private IEnumerable<GraphQLFieldConverter> GetFieldConverters(IEnumerable<GraphQLField> fields)
        {
            if (fields == null)
                yield break;

            foreach (var field in fields)
            {
                // Check if target types contains any elements
                if (field.TargetTypes?.Any() ?? false)
                {
                    yield return new GraphQLFieldConverter(field);
                }

                // Get fields in selection set
                if (field.SelectionSet != null)
                {
                    foreach (var fieldConverter in GetFieldConverters(field.SelectionSet))
                    {
                        yield return fieldConverter;
                    }
                }
            }
        }

        private class GraphQLFieldConverter : JsonConverter
        {
            private readonly GraphQLField graphQLField;

            public GraphQLFieldConverter(GraphQLField graphQLField)
            {
                this.graphQLField = graphQLField;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == graphQLField.BaseType;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                // Read object
                JObject jo = JObject.Load(reader);

                // Get typename
                var typeName = jo[Constants.TYPENAME_GRAPHQL_CONSTANT].Value<string>();

                // Find matching type
                foreach (var type in graphQLField.TargetTypes)
                {
                    // Compare types
                    if (string.Equals(typeName, type.Key, StringComparison.Ordinal))
                    {
                        return jo.ToObject(type.Value.Type, serializer);
                    }
                }

                return jo.ToObject(objectType, serializer);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
