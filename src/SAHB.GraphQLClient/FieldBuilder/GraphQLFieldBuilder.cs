using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.Exceptions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.Internal;

namespace SAHB.GraphQLClient.FieldBuilder
{
    // ReSharper disable once InconsistentNaming
    /// <inheritdoc />
    public class GraphQLFieldBuilder : IGraphQLFieldBuilder
    {
        /// <inheritdoc />
        public IEnumerable<IGraphQLField> GenerateSelectionSet(Type type)
        {
            // Get selectionSet
            var selectionSet = GetSelectionSet(type);
            return selectionSet;
        }

        [Obsolete("Please use GenerateSelectionSet instead")]
        /// <inheritdoc />
        public IEnumerable<IGraphQLField> GetFields(Type type) => GenerateSelectionSet(type);

        /// <inheritdoc />
        private IEnumerable<GraphQLField> GetSelectionSet(Type type)
        {
            // Initialize list with fields and arguments
            var fields = new List<GraphQLField>();

            // Get all properties which has a public get method and can read and write
            var properties = type.GetRuntimeProperties()
                .Where(e => e.CanRead && e.CanWrite && e.GetMethod.IsPublic);

            foreach (var property in properties)
            {
                // Check if property or property class is ignored
                if (TypeIgnoredMemberInfo(property))
                    continue;

                // Add field
                fields.Add(GetGraphQLField(property));
            }

            // Logging
            if (Logger != null && Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"Generated the following fields from the type {type.FullName}{Environment.NewLine}{String.Join(Environment.NewLine, fields)}");
            }

            return fields;
        }

        #region TypeIgnored
        private bool TypeIgnored(PropertyInfo propertyInfo)
        {
            // Check if property is ignored
            if (TypeIgnoredMemberInfo(propertyInfo))
                return true;

            // Check if the GraphQLClass is ignored
            if (IsSelectionSetIEnumerable(propertyInfo))
            {
                return TypeIgnoredMemberInfo(GetIEnumerableType(propertyInfo.PropertyType).GetTypeInfo());
            }
            else
            {
                return TypeIgnoredMemberInfo(propertyInfo);
            }
        }

        private bool TypeIgnoredMemberInfo(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttribute<GraphQLFieldIgnoreAttribute>() != null;
        }
        #endregion

        private GraphQLField GetGraphQLField(PropertyInfo property)
        {
            // Get alias and fieldName
            var alias = GetPropertyAlias(property);
            var fieldName = GetPropertyField(property);

            // Get arguments
            var arguments = GetPropertyArguments(property);

            // Get types
            // TODO: Possible problems if types is IEnumerable types
            var types = GetTypes(property)
                .Select(e => new { typeName = e.Key, field = (IGraphQLTargetType) new GraphQLTargetType(e.Value, GetSelectionSet(e.Value)) })
                .ToDictionary(e => e.typeName, e => e.field);

            // Get selectionSet
            IEnumerable<GraphQLField> selectionSet = null;
            if (ShouldIncludeSelectionSet(property))
            {
                if (IsSelectionSetIEnumerable(property))
                {
                    selectionSet = GetSelectionSet(GetIEnumerableType(property.PropertyType));
                }
                else
                {
                    selectionSet = GetSelectionSet(property.PropertyType);
                }
            }

            // Add __typename if multiple types
            if (types.Any())
            {
                // Check if selectionSet has been set
                if (selectionSet == null)
                {
                    throw new NotSupportedException($"Cannot add {Constants.TYPENAME_GRAPHQL_CONSTANT} to a type which does not have a selectionSet");
                }

                // Check if __typename is not already in the selected fields
                if (!selectionSet.Any(field => field.Field == Constants.TYPENAME_GRAPHQL_CONSTANT))
                {
                    selectionSet = selectionSet.Union(new List<GraphQLField>() { new GraphQLField(null, Constants.TYPENAME_GRAPHQL_CONSTANT, null, null) });
                }
            }

            // Return GraphQLField
            return new GraphQLField(alias, fieldName, selectionSet, arguments, property.PropertyType, types);
        }

        private bool ShouldIncludeSelectionSet(PropertyInfo property)
        {
            // Get the type of the property
            var propertyType = property.PropertyType;

            // Check if primitive or value type
            if (propertyType.GetTypeInfo().IsPrimitive || propertyType.GetTypeInfo().IsValueType)
                return false;

            // Check if the type is a IEnumerable type
            if (IsIEnumerableType(propertyType))
            {
                // Detect if the enumerable type is a System type (String is a IEnumerable type and should not include SelectionSet)
                if (GetIEnumerableType(propertyType).GetTypeInfo().Name.StartsWith(nameof(System), StringComparison.Ordinal))
                {
                    return false;
                }

                // Detect if the type of the IEnumerable is not a value type
                if (!GetIEnumerableType(propertyType).GetTypeInfo().IsValueType)
                {
                    return true;
                }
            }

            return true;
        }

        private bool IsSelectionSetIEnumerable(PropertyInfo property)
        {
            // Get the type of the property
            var propertyType = property.PropertyType;

            // Check if primitive or value type
            if (propertyType.GetTypeInfo().IsPrimitive || propertyType.GetTypeInfo().IsValueType)
                return false;

            // Check if the type is a IEnumerable type
            if (IsIEnumerableType(propertyType))
            {
                // Detect if the enumerable type is a System type (String is a IEnumerable type and should not include SelectionSet)
                if (GetIEnumerableType(propertyType).GetTypeInfo().Name.StartsWith(nameof(System), StringComparison.Ordinal))
                {
                    return false;
                }

                // Detect if the type of the IEnumerable is not a value type
                if (!GetIEnumerableType(propertyType).GetTypeInfo().IsValueType)
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual string GetPropertyAlias(PropertyInfo property)
        {
            return property.Name;
        }

        protected virtual IDictionary<string, Type> GetTypes(PropertyInfo property)
        {
            // Get GraphQLUnionOrInterfaceAttribute on field and class
            var attributes = property
                .GetCustomAttributes<GraphQLUnionOrInterfaceAttribute>()
                .Union(
                    property.PropertyType.GetTypeInfo().GetCustomAttributes<GraphQLUnionOrInterfaceAttribute>());
            
            // Check if dictionary contains duplicates
            var duplicates = attributes.Select(e => e.TypeName).GroupBy(e => e, e => e).Where(e => e.Count() > 1)
                .Select(e => e.Key).ToArray();
            if (duplicates.Any())
            {
                throw new GraphQLDuplicateTypeNameException(duplicates);
            }

            // Return dictionary
            return attributes.ToDictionary(attribute => attribute.TypeName, attribute => attribute.Type);
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

        protected virtual IEnumerable<IGraphQLArguments> GetPropertyArguments(PropertyInfo property)
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
                new GraphQLFieldArguments(attribute));
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
        ///     Gets type parameter from a the type <param name="typeInfo"></param> which inherits from <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <returns>Returns the type parameter from the <see cref="IEnumerable{T}" /></returns>
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

        #region Logging

        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// Contains a logger factory for the GraphQLHttpClient
        /// </summary>
        public ILoggerFactory LoggerFactory
        {
            internal get { return _loggerFactory; }
            set
            {
                _loggerFactory = value;
                if (_loggerFactory != null)
                {
                    Logger = _loggerFactory.CreateLogger<GraphQLFieldBuilder>();
                }
            }
        }

        /// <summary>
        /// Contains the logger for the class
        /// </summary>
        private ILogger<GraphQLFieldBuilder> Logger { get; set; }

        #endregion
    }
}