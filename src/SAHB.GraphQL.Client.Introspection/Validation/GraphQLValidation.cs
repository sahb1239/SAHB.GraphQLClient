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
    public static class GraphQLValidation
    {
        public static IEnumerable<ValidationOutput> ValidateGraphQLSelectionSet(this GraphQLIntrospectionSchema graphQLIntrospectionSchema, GraphQLOperationType operationType, IEnumerable<GraphQLField> selectionSet)
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
                return new List<ValidationOutput> {
                    new ValidationOutput(ValidationType.Operation_Type_Not_Found, operationType)
                };
            }

            return ValidateSelectionSet(graphQLIntrospectionSchemaWithImplicitTypes, selectionSet, type, operationType, rootLevel: true);
        }

        private static IEnumerable<ValidationOutput> ValidateSelectionSet(GraphQLIntrospectionSchema graphQLIntrospectionSchema, IEnumerable<GraphQLField> selectionSet, GraphQLIntrospectionFullType graphQLIntrospectionType, GraphQLOperationType operationType, string fieldPath = null, bool rootLevel = false)
        {
            foreach (var selection in selectionSet)
            {
                // Get fieldPath
                var selectionFieldPath = fieldPath == null ? selection.Field : string.Join(".", fieldPath, selection.Field);

                // Get field from introspection
                var introspectionField = graphQLIntrospectionType.Fields.SingleOrDefault(e => e.Name == selection.Field);
                if (introspectionField == null)
                {
                    yield return new ValidationOutput(selectionFieldPath, ValidationType.Field_Not_Found, selection);
                    continue;
                }

                // IsDeprecated
                if (introspectionField.IsDeprecated)
                {
                    yield return new ValidationOutput(selectionFieldPath, ValidationType.Field_Deprecated, selection);
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
                        yield return new ValidationOutput(selectionFieldPath, ValidationType.Argument_Not_Found, selection);
                        continue;
                    }

                    // Validate type
                    if (!string.Equals(introspectionArgument.Type.Name, argument.ArgumentType, StringComparison.OrdinalIgnoreCase))
                    {
                        yield return new ValidationOutput(selectionFieldPath, ValidationType.Argument_Invalid_Type, selection, introspectionArgument.Type.Name, argument.ArgumentType);
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
                            yield return new ValidationOutput(selectionFieldPath, ValidationType.Field_Cannot_Have_SelectionSet, selection);
                        }
                        break;
                    case GraphQLTypeKind.Object:
                    case GraphQLTypeKind.Interface:
                        // Object/Interface should have selectionSet
                        if (!hasSelectionSet)
                        {
                            yield return new ValidationOutput(selectionFieldPath, ValidationType.Field_Should_Have_SelectionSet, selection);
                        }
                        break;
                    default:
                        // Report error
                        throw new NotImplementedException($"{nameof(GraphQLTypeKind)} {introspectionField.Type.Kind} not implemented for fields");
                }

                // Validate type
                if (selection.BaseType != null)
                {
                    switch (graphQLType.Kind)
                    {
                        case GraphQLTypeKind.Enum:
                            foreach (var error in ValidateEnum(selectionFieldPath, selection, IsListType(introspectionField.Type), graphQLType))
                            {
                                yield return error;
                            }
                            break;
                        case GraphQLTypeKind.Scalar:
                            foreach (var error in ValidateScalar(selectionFieldPath, selection, IsListType(introspectionField.Type), graphQLType))
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
                            yield return new ValidationOutput(possibleTypeFieldPath, ValidationType.PossibleType_Not_Found, selection);
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

        private static IEnumerable<ValidationOutput> ValidateScalar(string selectionFieldPath, GraphQLField selection, bool isListType, GraphQLIntrospectionFullType graphQLType)
        {
            var type = selection.BaseType;

            if (isListType)
            {
                type = GetIEnumerableType(type);
            }

            switch (graphQLType.Name)
            {
                case "String":
                    if (type != typeof(string))
                    {
                        yield return new ValidationOutput(
                            selectionFieldPath, 
                            ValidationType.Type_Is_Invalid, 
                            selection, 
                            typeof(string).Name, 
                            type.Name);
                    }
                    break;
                case "Boolean":
                    if (type != typeof(bool))
                    {
                        yield return new ValidationOutput(
                            selectionFieldPath, 
                            ValidationType.Type_Is_Invalid, 
                            selection, 
                            typeof(bool).Name, 
                            type.Name);
                    }
                    break;
                case "Float":
                case "Decimal":
                    if (type != typeof(float) 
                        && type != typeof(double) 
                        && type != typeof(decimal))
                    {
                        yield return new ValidationOutput(
                            selectionFieldPath, 
                            ValidationType.Type_Is_Invalid, 
                            selection, 
                            $"{typeof(float).Name} or {typeof(double).Name} or {typeof(decimal).Name}", 
                            type.Name);
                    }
                    break;
            }
        }

        private static IEnumerable<ValidationOutput> ValidateEnum(string selectionFieldPath, GraphQLField selection, bool isListType, GraphQLIntrospectionFullType graphQLType)
        {
            var type = selection.BaseType;

            if (isListType)
            {
                type = GetIEnumerableType(type);
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
                            yield return new ValidationOutput($"{selectionFieldPath}[{enumMemberValue}]", ValidationType.EnumValue_Not_Found, selection);
                            continue;
                        }

                        // Validate that if the enum member is deprecated
                        if (enumValue.IsDeprecated)
                        {
                            yield return new ValidationOutput($"{selectionFieldPath}[{enumMemberValue}]", ValidationType.EnumValue_Deprecated, selection);
                            continue;
                        }
                    }
                }
            }
            else
            {
                yield return new ValidationOutput(selectionFieldPath, ValidationType.Type_Is_Not_Enum, selection);
            }
        }

        private static bool IsListType(GraphQLIntrospectionTypeRef graphQLIntrospectionTypeRef)
        {
            if (graphQLIntrospectionTypeRef.Kind == GraphQLTypeKind.List)
                return true;

            if (HasSubtype(graphQLIntrospectionTypeRef.Kind))
            {
                var ofType1 = graphQLIntrospectionTypeRef.OfType;

                if (ofType1.Kind == GraphQLTypeKind.List)
                    return true;

                if (HasSubtype(ofType1.Kind))
                {
                    var ofType2 = ofType1.OfType;

                    if (ofType2.Kind == GraphQLTypeKind.List)
                        return true;

                    if (HasSubtype(ofType2.Kind))
                    {
                        var ofType3 = ofType2.OfType;

                        if (ofType3.Kind == GraphQLTypeKind.List)
                            return true;

                        if (HasSubtype(ofType3.Kind))
                        {
                            var ofType4 = ofType3.OfType;

                            if (ofType4.Kind == GraphQLTypeKind.List)
                                return true;

                            if (HasSubtype(ofType4.Kind))
                            {
                                var ofType5 = ofType4.OfType;

                                if (ofType5.Kind == GraphQLTypeKind.List)
                                    return true;

                                if (HasSubtype(ofType5.Kind))
                                {
                                    var ofType6 = ofType5.OfType;

                                    if (ofType6.Kind == GraphQLTypeKind.List)
                                        return true;

                                    if (HasSubtype(ofType6.Kind))
                                    {
                                        var ofType7 = ofType6.OfType;
                                        if (ofType7.Kind == GraphQLTypeKind.List)
                                            return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static GraphQLIntrospectionFullType GetSubtype(GraphQLIntrospectionSchema graphQLIntrospectionSchema, GraphQLIntrospectionTypeRef graphQLIntrospectionTypeRef)
        {
            GraphQLIntrospectionFullType graphQLType;

            if (HasSubtype(graphQLIntrospectionTypeRef.Kind))
            {
                var ofType1 = graphQLIntrospectionTypeRef.OfType;
                if (HasSubtype(ofType1.Kind))
                {
                    var ofType2 = ofType1.OfType;
                    if (HasSubtype(ofType2.Kind))
                    {
                        var ofType3 = ofType2.OfType;
                        if (HasSubtype(ofType3.Kind))
                        {
                            var ofType4 = ofType3.OfType;
                            if (HasSubtype(ofType4.Kind))
                            {
                                var ofType5 = ofType4.OfType;
                                if (HasSubtype(ofType5.Kind))
                                {
                                    var ofType6 = ofType5.OfType;
                                    if (HasSubtype(ofType6.Kind))
                                    {
                                        var ofType7 = ofType6.OfType;
                                        if (HasSubtype(ofType7.Kind))
                                        {
                                            throw new NotImplementedException();
                                        }
                                        else
                                        {
                                            graphQLType = GetTypeByName(graphQLIntrospectionSchema, ofType7.Name);
                                        }
                                    }
                                    else
                                    {
                                        graphQLType = GetTypeByName(graphQLIntrospectionSchema, ofType6.Name);
                                    }
                                }
                                else
                                {
                                    graphQLType = GetTypeByName(graphQLIntrospectionSchema, ofType5.Name);
                                }
                            }
                            else
                            {
                                graphQLType = GetTypeByName(graphQLIntrospectionSchema, ofType4.Name);
                            }
                        }
                        else
                        {
                            graphQLType = GetTypeByName(graphQLIntrospectionSchema, ofType3.Name);
                        }
                    }
                    else
                    {
                        graphQLType = GetTypeByName(graphQLIntrospectionSchema, ofType2.Name);
                    }
                }
                else
                {
                    graphQLType = GetTypeByName(graphQLIntrospectionSchema, ofType1.Name);
                }
            }
            else
            {
                graphQLType = GetTypeByName(graphQLIntrospectionSchema, graphQLIntrospectionTypeRef.Name);
            }
            return graphQLType;
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
