using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    public class GraphQLFieldBuilder : IGraphQLFieldBuilder
    {
        /// <inheritdoc />
        public IEnumerable<GraphQLField> GetFields(Type type)
        {
            // Initilize list with fields and arguments
            var fields = new List<GraphQLField>();

            // Get all properties which has a public get method and can read and write
            var properties = type.GetRuntimeProperties()
                .Where(e => e.CanRead && e.CanWrite && e.GetMethod.IsPublic);

            foreach (var property in properties)
            {
                // Check if property is ignored
                if (TypeIgnored(property))
                    continue;

                // Get the type of the property
                var propertyType = property.PropertyType;

                // Check if primitive type or ValueType
                if (propertyType.GetTypeInfo().IsPrimitive || propertyType.GetTypeInfo().IsValueType)
                {
                    // Check if class is ignored
                    if (TypeIgnored(propertyType))
                        continue;

                    fields.Add(GetGraphQLField(type, property));
                }
                // Check if IEnumerable type and the enumerable type is not a System type (String is a IEnumerable type and therefore would otherwise go into this case)
                else if (IsIEnumerableType(propertyType) &&
                         !GetIEnumerableType(propertyType).GetTypeInfo().IsValueType &&
                         !GetIEnumerableType(propertyType).GetTypeInfo().Name.StartsWith(nameof(System), StringComparison.Ordinal))
                {
                    // Check if class is ignored
                    if (TypeIgnored(GetIEnumerableType(propertyType)))
                        continue;

                    fields.Add(GetGraphQLIEnumerableType(type, property));
                }
                // Check if System type
                else if (propertyType.GetTypeInfo().Namespace.StartsWith(nameof(System), StringComparison.Ordinal))
                {
                    // Check if class is ignored
                    if (TypeIgnored(propertyType))
                        continue;

                    fields.Add(GetGraphQLField(type, property));
                }
                // Else return subtypes
                else
                {
                    // Check if class is ignored
                    if (TypeIgnored(propertyType))
                        continue;

                    fields.Add(GetGraphQLFieldWithSubfields(type, property));
                }
            }

            // Return fields
            return fields;
        }

        private bool TypeIgnored(Type type) => TypeIgnored(type.GetTypeInfo());

        private bool TypeIgnored(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttribute<GraphQLFieldIgnoreAttribute>() != null;
        }
        
        private GraphQLField GetGraphQLField(Type type, PropertyInfo property)
        {
            return new GraphQLField(GetPropertyAlias(property), GetPropertyField(property), null,
                GetPropertyArguments(property));
        }

        private GraphQLField GetGraphQLFieldWithSubfields(Type type, PropertyInfo property)
        {
            return new GraphQLField(GetPropertyAlias(property), GetPropertyField(property),
                GetFields(property.PropertyType), GetPropertyArguments(property));
        }

        private GraphQLField GetGraphQLIEnumerableType(Type type, PropertyInfo property)
        {
            return new GraphQLField(GetPropertyAlias(property), GetPropertyField(property),
                GetFields(GetIEnumerableType(property.PropertyType)), GetPropertyArguments(property));
        }

        protected virtual string GetPropertyAlias(PropertyInfo property)
        {
            return property.Name;
        }

        protected virtual string GetPropertyField(PropertyInfo property)
        {
            // Get GraphQLFieldNameAttribute on field
            var fieldAttribute = property.GetCustomAttribute<GraphQLFieldNameAttribute>();

            // If has GraphQLFieldNameAttribute on property
            if (fieldAttribute != null)
                return fieldAttribute.FieldName;

            // Get GraphQLFieldNameAttribute on class
            var propertyType = property.PropertyType;
            var classAttribute = propertyType.GetTypeInfo().GetCustomAttribute<GraphQLFieldNameAttribute>();

            // If has GraphQLFieldNameAttribute on class
            if (classAttribute != null)
                return classAttribute.FieldName;

            // Detect if it's a IEnumerable type
            if (IsIEnumerableType(propertyType) &&
                !GetIEnumerableType(propertyType).GetTypeInfo().IsValueType &&
                !GetIEnumerableType(propertyType).GetTypeInfo().Name.StartsWith(nameof(System), StringComparison.Ordinal))
            {
                classAttribute = GetIEnumerableType(propertyType).GetTypeInfo()
                    .GetCustomAttribute<GraphQLFieldNameAttribute>();

                // If has GraphQLFieldNameAttribute on class
                if (classAttribute != null)
                    return classAttribute.FieldName;
            }

            // Return camelCase as default
            if (property.Name.Length > 1)
                return property.Name.First().ToString().ToLower() + property.Name.Substring(1);
            return property.Name.ToLower();
        }

        protected virtual IEnumerable<GraphQLFieldArguments> GetPropertyArguments(PropertyInfo property)
        {
            // Get GraphQLArgumentsAttribute on class
            var propertyType = property.PropertyType;
            var classAttributes = propertyType.GetTypeInfo().GetCustomAttributes<GraphQLArgumentsAttribute>().ToList();

            // Check if the property type is IEnumerable type
            if (IsIEnumerableType(propertyType))
            {
                // Get attributes for type
                var attributes = GetIEnumerableType(propertyType).GetTypeInfo()
                    .GetCustomAttributes<GraphQLArgumentsAttribute>().ToList();

                // Add attributes
                classAttributes = classAttributes.Concat(attributes).ToList();
            }

            // Get GraphQLArgumentsAttribute on field
            var fieldAttribute = property.GetCustomAttributes<GraphQLArgumentsAttribute>().ToList();

            // If no attributes was found
            if (!classAttributes.Any() && !fieldAttribute.Any())
                return null;

            return classAttributes.Concat(fieldAttribute).Select(attribute =>
                new GraphQLFieldArguments(attribute.ArgumentName, attribute.ArgumentType, attribute.VariableName));
        }

        #region Helpers

        private static bool IsIEnumerableType(Type typeInfo)
        {
            // Check if the type is a array
            if (typeInfo.IsArray)
                return true;

            // Check if the type is a IEnumerable<>
            if (typeInfo.IsConstructedGenericType &&
                typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return true;

            // Check if the type is a IAsyncEnumerable<>
            if (typeInfo.IsConstructedGenericType &&
                typeInfo.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
                return true;

            // Get the first implemented interface which is the type IEnumerable<>
            var interfacesImplemented = typeInfo.GetTypeInfo().ImplementedInterfaces
                .Select(t => t.GetTypeInfo())
                .FirstOrDefault(IsGenericIEnumerable);

            if (interfacesImplemented != null)
                return true;

            // Get the first implemented interface which is the type IAsyncEnumerable<>
            interfacesImplemented = typeInfo.GetTypeInfo().ImplementedInterfaces
                .Select(t => t.GetTypeInfo())
                .FirstOrDefault(IsGenericIAsyncEnumerable);

            if (interfacesImplemented != null)
                return true;

            return false;
        }

        /// <summary>
        ///     Gets type parameter from a <see cref="IEnumerable{T}" /> type <see cref="T" />
        /// </summary>
        /// <typeparam name="T">The <see cref="IEnumerable{T}" /> type</typeparam>
        /// <returns>Returns the typeparameter T from the <see cref="IEnumerable{T}" /></returns>
        private static Type GetIEnumerableType(Type typeInfo)
        {
            // Check if the type is a array
            if (typeInfo.IsArray)
                return typeInfo.GetElementType();

            // Check if the type is a IEnumerable<>
            if (typeInfo.IsConstructedGenericType &&
                typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return typeInfo.GenericTypeArguments.First();

            // Check if the type is a IAsyncEnumerable<>
            if (typeInfo.IsConstructedGenericType &&
                typeInfo.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
                return typeInfo.GenericTypeArguments.First();

            // Get the first implemented interface which is the type IEnumerable<>
            var interfacesImplemented = typeInfo.GetTypeInfo().ImplementedInterfaces
                .Select(t => t.GetTypeInfo())
                .FirstOrDefault(IsGenericIEnumerable);

            if (interfacesImplemented != null)
                return interfacesImplemented.GenericTypeArguments.First();

            // Get the first implemented interface which is the type IAsyncEnumerable<>
            interfacesImplemented = typeInfo.GetTypeInfo().ImplementedInterfaces
                .Select(t => t.GetTypeInfo())
                .FirstOrDefault(IsGenericIAsyncEnumerable);

            if (interfacesImplemented != null)
                return interfacesImplemented.GenericTypeArguments.First();

            throw new NotSupportedException(
                $"The type {typeInfo.FullName} is not supported. It should be a IEnumerable<T> type");
        }

        private static bool IsIEnumerable(TypeInfo type)
        {
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type);
        }

        private static bool IsGenericIEnumerable(TypeInfo enumerableType)
        {
            return IsIEnumerable(enumerableType)
                   && enumerableType.IsGenericType
                   && enumerableType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static bool IsGenericIAsyncEnumerable(TypeInfo enumerableType)
        {
            return enumerableType.IsGenericType
                   && enumerableType.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>);
        }

        #endregion
    }
}