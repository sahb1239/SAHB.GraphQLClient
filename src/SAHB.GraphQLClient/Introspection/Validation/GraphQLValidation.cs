using SAHB.GraphQL.Client.Introspection.Extentions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace SAHB.GraphQL.Client.Introspection.Validation
{
    /// <inheritdoc />
    public class GraphQLValidation : IGraphQLValidation
    {
        /// <summary>
        /// Validate if the GraphQL <paramref name="selectionSet"/> is valid for the specified <paramref name="graphQLIntrospectionSchema"/> and <paramref name="operationType"/>
        /// </summary>
        /// <param name="graphQLIntrospectionSchema">The introspectionSchema to validate against</param>
        /// <param name="operationType">The operationType to validate against</param>
        /// <param name="selectionSet">The selectionSet which should be validated</param>
        /// <returns>An empty list if no errors were found or a <see cref="ValidationError"/> for each error found</returns>
        public IEnumerable<ValidationError> ValidateGraphQLSelectionSet(GraphQLIntrospectionSchema graphQLIntrospectionSchema, GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet)
        {
            if (graphQLIntrospectionSchema is null)
            {
                throw new ArgumentNullException(nameof(graphQLIntrospectionSchema));
            }

            if (selectionSet is null)
            {
                throw new ArgumentNullException(nameof(selectionSet));
            }

            // Get implicit types
            var graphQLIntrospectionSchemaWithImplicitTypes = graphQLIntrospectionSchema.WithImplicitFields();

            GraphQLIntrospectionFullType type = null;
            switch (operationType)
            {
                case GraphQLOperationType.Query:
                    if (graphQLIntrospectionSchemaWithImplicitTypes.QueryType != null)
                    {
                        type = GetTypeByName(graphQLIntrospectionSchemaWithImplicitTypes, graphQLIntrospectionSchemaWithImplicitTypes.QueryType.Name);
                    }
                    break;
                case GraphQLOperationType.Mutation:
                    if (graphQLIntrospectionSchemaWithImplicitTypes.MutationType != null)
                    {
                        type = GetTypeByName(graphQLIntrospectionSchemaWithImplicitTypes, graphQLIntrospectionSchemaWithImplicitTypes.MutationType.Name);
                    }
                    break;
                case GraphQLOperationType.Subscription:
                    if (graphQLIntrospectionSchemaWithImplicitTypes.SubscriptionType != null)
                    {
                        type = GetTypeByName(graphQLIntrospectionSchemaWithImplicitTypes, graphQLIntrospectionSchemaWithImplicitTypes.SubscriptionType.Name);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operationType), $"Operationtype {operationType} is not supported");
            }

            if (type == null)
            {
                return new List<ValidationError> {
                    new ValidationError(ValidationType.Operation_Type_Not_Found, operationType)
                };
            }

            return ValidateSelectionSet(graphQLIntrospectionSchemaWithImplicitTypes, selectionSet, type, operationType, rootLevel: true);
        }

        private static IEnumerable<ValidationError> ValidateSelectionSet(GraphQLIntrospectionSchema graphQLIntrospectionSchema, IEnumerable<GraphQLField> selectionSet, GraphQLIntrospectionFullType graphQLIntrospectionType, GraphQLOperationType operationType, string fieldPath = null, bool rootLevel = false)
        {
            foreach (var selection in selectionSet)
            {
                // Get fieldPath
                var selectionFieldPath = fieldPath == null ? selection.Field : string.Join(".", fieldPath, selection.Field);

                // Get field from introspection
                var introspectionField = graphQLIntrospectionType.Fields.SingleOrDefault(e => e.Name == selection.Field);
                if (introspectionField == null)
                {
                    yield return new ValidationError(selectionFieldPath, ValidationType.Field_Not_Found, selection);
                    continue;
                }

                // IsDeprecated
                if (introspectionField.IsDeprecated)
                {
                    yield return new ValidationError(selectionFieldPath, ValidationType.Field_Deprecated, selection);
                }

                // Validate arguments
                foreach (var argument in selection.Arguments)
                {
                    // Get argument path
                    var argumentPath = $"{selectionFieldPath}({argument.ArgumentName})";

                    // Get argument
                    var introspectionArgument = introspectionField.Args.SingleOrDefault(arg => arg.Name == argument.ArgumentName);

                    if (introspectionArgument == null)
                    {
                        yield return new ValidationError(selectionFieldPath, ValidationType.Argument_Not_Found, selection);
                        continue;
                    }

                    // Validate type
                    var typeName = GetTypeName(introspectionArgument.Type);
                    if (!string.Equals(typeName, argument.ArgumentType, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return new ValidationError(selectionFieldPath, ValidationType.Argument_Invalid_Type, selection, introspectionArgument.Type.Name, argument.ArgumentType);
                    }
                }

                // Switch on type kind (ignore __typename since union can only have __typename as selectionSet)
                var hasSelectionSet = selection.SelectionSet.Any(f => f.Field != "__typename");

                // Get concrete type
                GraphQLIntrospectionFullType graphQLType = GetSubtype(graphQLIntrospectionSchema, introspectionField.Type);

                // Get kind
                switch (graphQLType.Kind)
                {
                    case GraphQLTypeKind.Scalar:
                    case GraphQLTypeKind.Union:
                    case GraphQLTypeKind.Enum:
                        // If Scalar/Union/Enum we cannot have selectionSet
                        if (hasSelectionSet)
                        {
                            yield return new ValidationError(selectionFieldPath, ValidationType.Field_Cannot_Have_SelectionSet, selection);
                        }
                        break;
                    case GraphQLTypeKind.Object:
                    case GraphQLTypeKind.Interface:
                        // Object/Interface should have selectionSet
                        if (!hasSelectionSet)
                        {
                            yield return new ValidationError(selectionFieldPath, ValidationType.Field_Should_Have_SelectionSet, selection);
                        }
                        break;
                    default:
                        // Report error
                        throw new NotImplementedException($"{nameof(GraphQLTypeKind)} {introspectionField.Type.Kind} not implemented for fields");
                }

                // TODO: Validation should also validate type is correct for instance if GraphQLString is having the type String

                // Validate type
                if (selection.BaseType != null)
                {
                    switch (graphQLType.Kind)
                    {
                        case GraphQLTypeKind.Enum:
                            foreach (var error in ValidateEnum(selectionFieldPath, selection, IsListType(introspectionField.Type), IsNonNull(introspectionField.Type), graphQLType))
                            {
                                yield return error;
                            }
                            break;
                        case GraphQLTypeKind.Scalar:
                            foreach (var error in ValidateScalar(selectionFieldPath, selection, IsListType(introspectionField.Type), IsNonNull(introspectionField.Type), graphQLType))
                            {
                                yield return error;
                            }
                            break;
                    }
                }

                // Validate selectionSet
                if (hasSelectionSet)
                {
                    // Validate selectionSet
                    foreach (var result in
                        ValidateSelectionSet(graphQLIntrospectionSchema, selection.SelectionSet, graphQLType, operationType, selectionFieldPath))
                    {
                        yield return result;
                    }

                    // Validate possible types
                    foreach (var possibleType in selection.TargetTypes)
                    {
                        // Get fieldPath for possible type
                        var possibleTypeFieldPath = $"{selectionFieldPath}[{possibleType.Key}]";

                        // Get type
                        var introspectionPossibleType = GetTypeByName(graphQLIntrospectionSchema, possibleType.Key);
                        if (introspectionPossibleType == null)
                        {
                            yield return new ValidationError(possibleTypeFieldPath, ValidationType.PossibleType_Not_Found, selection);
                            continue;
                        }

                        // Validate selectionSet
                        foreach (var result in
                            ValidateSelectionSet(graphQLIntrospectionSchema, possibleType.Value.SelectionSet, introspectionPossibleType, operationType, possibleTypeFieldPath))
                        {
                            yield return result;
                        }
                    }
                }
            }
        }
        
        private static string GetTypeName(GraphQLIntrospectionTypeRef type)
        {
            switch (type.Kind)
            {
                case GraphQLTypeKind.NonNull:
                    return $"{GetTypeName(type.OfType)}!";
                case GraphQLTypeKind.List:
                    return $"[{GetTypeName(type.OfType)}]";
                default:
                    return type.Name;
            }
        }

        private static IEnumerable<ValidationError> ValidateScalar(string selectionFieldPath, GraphQLField selection, bool isListType, bool isNonNull, GraphQLIntrospectionFullType graphQLType)
        {
            var type = selection.BaseType;

            if (isListType)
            {
                type = GetIEnumerableType(type);
            }

            switch (graphQLType.Name)
            {
                case "String":
                    if (!IsValidStringType(type))
                    {
                        yield return new ValidationError(
                            selectionFieldPath,
                            ValidationType.Field_Invalid_Type,
                            selection,
                            typeof(string).Name,
                            type.Name);
                    }
                    break;
                case "Boolean":
                    if (!IsValidBooleanType(type, isNonNull))
                    {
                        yield return new ValidationError(
                            selectionFieldPath,
                            ValidationType.Field_Invalid_Type,
                            selection,
                            typeof(bool).Name,
                            type.Name);
                    }
                    break;
                case "Float":
                case "Decimal":
                    if (!IsValidFloatOrDecimalType(type, isNonNull))
                    {
                        yield return new ValidationError(
                            selectionFieldPath,
                            ValidationType.Field_Invalid_Type,
                            selection,
                            $"{typeof(float).Name} or {typeof(double).Name} or {typeof(decimal).Name}",
                            type.Name);
                    }
                    break;
            }
        }

        private static bool IsValidStringType(Type type)
        {
            return type == typeof(string);
        }

        private static bool IsValidBooleanType(Type type, bool isNonNull)
        {
            return type == typeof(bool)
                || (!isNonNull && type == typeof(bool?));
        }

        private static bool IsValidFloatOrDecimalType(Type type, bool isNonNull)
        {
            return type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal)
                || (!isNonNull && type == typeof(float?))
                || (!isNonNull && type == typeof(double?))
                || (!isNonNull && type == typeof(decimal?));
        }

        private static IEnumerable<ValidationError> ValidateEnum(string selectionFieldPath, GraphQLField selection, bool isListType, bool isNonNull, GraphQLIntrospectionFullType graphQLType)
        {
            var type = selection.BaseType;

            if (isListType)
            {
                type = GetIEnumerableType(type);
            }

            var isNullable = IsNullableType(type);
            if (isNullable)
            {
                type = type.GenericTypeArguments.First();
            }

            // Validate nullable
            if (!isNullable != isNonNull)
            {
                if (isNonNull)
                {
                    yield return new ValidationError(selectionFieldPath, ValidationType.Field_Should_Be_NonNull, selection);
                }
                else
                {
                    yield return new ValidationError(selectionFieldPath, ValidationType.Field_Should_Be_Nullable, selection);
                }
            }

            if (type.GetTypeInfo().IsEnum)
            {
                // Get all enum names
                var memberNames = Enum.GetNames(type);

                // Get enum values
                foreach (var member in type.GetTypeInfo().DeclaredMembers)
                {
                    if (memberNames.Contains(member.Name))
                    {
                        var enumMemberValue = member.Name;

                        // Detect if an attribute overrides the name
                        var attribute = member.GetCustomAttribute<EnumMemberAttribute>();
                        if (attribute != null)
                        {
                            enumMemberValue = attribute.Value;
                        }

                        // Validate if the enum member could be found
                        var enumValue = graphQLType.EnumValues.SingleOrDefault(introspectionMember => string.Equals(introspectionMember.Name, enumMemberValue, StringComparison.OrdinalIgnoreCase));
                        if (enumValue == null)
                        {
                            yield return new ValidationError($"{selectionFieldPath}[{enumMemberValue}]", ValidationType.EnumValue_Not_Found, selection);
                            continue;
                        }

                        // Validate that if the enum member is deprecated
                        if (enumValue.IsDeprecated)
                        {
                            yield return new ValidationError($"{selectionFieldPath}[{enumMemberValue}]", ValidationType.EnumValue_Deprecated, selection);
                            continue;
                        }
                    }
                }
            }
            else
            {
                yield return new ValidationError(selectionFieldPath, ValidationType.Field_Type_Not_Enum, selection);
            }
        }

        private static bool IsNullableType(Type type)
        {
            var typeinfo = type.GetTypeInfo();
            if (typeinfo.GenericTypeArguments.Length != 1)
                return false;

            var genericType = typeinfo.GetGenericTypeDefinition();
            return genericType == typeof(Nullable<>);
        }

        private static bool IsListType(GraphQLIntrospectionTypeRef type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.Kind == GraphQLTypeKind.List)
                return true;

            if (HasSubtype(type.Kind))
            {
                return IsListType(type.OfType);
            }

            return false;
        }

        private static bool IsNonNull(GraphQLIntrospectionTypeRef type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (IsListType(type))
            {
                return IsNonNull(type.OfType);
            }
            else
            {
                return type.Kind == GraphQLTypeKind.NonNull;
            }
        }

        private static GraphQLIntrospectionFullType GetSubtype(GraphQLIntrospectionSchema graphQLIntrospectionSchema, GraphQLIntrospectionTypeRef graphQLIntrospectionTypeRef)
        {
            if (graphQLIntrospectionSchema is null)
            {
                throw new ArgumentNullException(nameof(graphQLIntrospectionSchema));
            }

            if (graphQLIntrospectionTypeRef is null)
            {
                throw new ArgumentNullException(nameof(graphQLIntrospectionTypeRef));
            }

            if (HasSubtype(graphQLIntrospectionTypeRef.Kind))
            {
                return GetSubtype(graphQLIntrospectionSchema, graphQLIntrospectionTypeRef.OfType);
            }

            return GetTypeByName(graphQLIntrospectionSchema, graphQLIntrospectionTypeRef.Name);
        }
        private static bool HasSubtype(GraphQLTypeKind kind)
        {
            return kind == GraphQLTypeKind.NonNull || kind == GraphQLTypeKind.List;
        }

        private static GraphQLIntrospectionFullType GetTypeByName(GraphQLIntrospectionSchema graphQLIntrospectionSchema, string name)
        {
            return graphQLIntrospectionSchema.Types.SingleOrDefault(type => string.Equals(type.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        #region GetIEnumerableType
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
    }
}
