using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using System;
using System.Collections.Generic;
using System.Linq;

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

            GraphQLIntrospectionFullType type = null;
            switch (operationType)
            {
                case GraphQLOperationType.Query:
                    if (graphQLIntrospectionSchema.QueryType != null)
                    {
                        type = GetTypeByName(graphQLIntrospectionSchema, graphQLIntrospectionSchema.QueryType.Name);
                    }
                    break;
                case GraphQLOperationType.Mutation:
                    if (graphQLIntrospectionSchema.MutationType != null)
                    {
                        type = GetTypeByName(graphQLIntrospectionSchema, graphQLIntrospectionSchema.MutationType.Name);
                    }
                    break;
                case GraphQLOperationType.Subscription:
                    if (graphQLIntrospectionSchema.SubscriptionType != null)
                    {
                        type = GetTypeByName(graphQLIntrospectionSchema, graphQLIntrospectionSchema.SubscriptionType.Name);
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

            return ValidateSelectionSet(graphQLIntrospectionSchema, selectionSet, type);
        }

        private static IEnumerable<ValidationOutput> ValidateSelectionSet(GraphQLIntrospectionSchema graphQLIntrospectionSchema, IEnumerable<GraphQLField> selectionSet, GraphQLIntrospectionFullType graphQLIntrospectionType, string fieldPath = null)
        {
            foreach (var selection in selectionSet)
            {
                // Get fieldPath
                var selectionFieldPath = fieldPath == null ? selection.Field : string.Join(".", fieldPath, selection.Field);

                // Get field from introspection
                var introspectionField = graphQLIntrospectionType.Fields.SingleOrDefault(e => e.Name == selection.Field);
                if (introspectionField == null)
                {
                    // __typename is a default field
                    if (selection.Field != "__typename")
                    { 
                        yield return new ValidationOutput(selectionFieldPath, ValidationType.Field_Not_Found, selection);
                    }
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

                // Switch on type kind (typename is ignored since this is the only field which is supported for union)
                var hasSelectionSet = selection.SelectionSet.Any(s => s.Field != "__typename");

                // Get kind
                var typeKind = introspectionField.Type.Kind;
                if (typeKind == GraphQLTypeKind.NonNull || typeKind == GraphQLTypeKind.List)
                {
                    typeKind = introspectionField.Type.OfType.Kind;
                }

                switch (typeKind)
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

                // Validate selectionSet
                if (hasSelectionSet)
                {
                    // Get type of field
                    var introspectionFieldType = GetTypeByName(graphQLIntrospectionSchema, introspectionField.Name);

                    // Validate selectionSet
                    foreach (var result in 
                        ValidateSelectionSet(graphQLIntrospectionSchema, selection.SelectionSet, introspectionFieldType, selectionFieldPath))
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
                            ValidateSelectionSet(graphQLIntrospectionSchema, possibleType.Value.SelectionSet, introspectionPossibleType, possibleTypeFieldPath))
                        {
                            yield return result;
                        }
                    }
                }
            }
        }

        private static GraphQLIntrospectionFullType GetTypeByName(GraphQLIntrospectionSchema graphQLIntrospectionSchema, string name)
        {
            return graphQLIntrospectionSchema.Types.SingleOrDefault(type => string.Equals(type.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
