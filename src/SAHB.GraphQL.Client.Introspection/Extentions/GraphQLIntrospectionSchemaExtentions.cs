using SAHB.GraphQLClient.Introspection;
using System.Collections.Generic;
using System.Linq;

namespace SAHB.GraphQL.Client.Introspection.Extentions
{
    public static class GraphQLIntrospectionSchemaExtentions
    {
        private const string Implicit_TypeName_Name = "__typename";
        private const string Implicit_TypeName_Type = "String";
        private const string Implicit_Schema_Name = "__schema";
        private const string Implicit_Schema_Type = "__Schema";
        private const string Implicit_Type_Name = "__type";
        private const string Implicit_Type_Type = "__Type";
        private const string Implicit_Type_Argument_Name = "name";
        private const string Implicit_Type_Argument_Type = "String";

        public static GraphQLIntrospectionSchema WithImplicitFields(this GraphQLIntrospectionSchema schema)
        {
            return new GraphQLIntrospectionSchema
            {
                QueryType = schema.QueryType,
                MutationType = schema.MutationType,
                SubscriptionType = schema.SubscriptionType,
                Directives = schema.Directives,
                Types = schema.Types.Select(type => GetIntrospectionType(type, schema))
            };
        }

        private static GraphQLIntrospectionFullType GetIntrospectionType(GraphQLIntrospectionFullType graphQLType, GraphQLIntrospectionSchema schema)
        {
            List<GraphQLIntrospectionField> fields = new List<GraphQLIntrospectionField>();
            if (graphQLType.Fields != null)
            {
                fields.AddRange(graphQLType.Fields);
            }

            // If it's the query type
            if (graphQLType.Name == schema.QueryType.Name)
            {
                // Add schema
                fields.Add(new GraphQLIntrospectionField
                {
                    Name = Implicit_Schema_Name,
                    Type = new GraphQLIntrospectionTypeRef
                    {
                        Kind = GraphQLTypeKind.NonNull,
                        OfType = new GraphQLIntrospectionTypeRef<GraphQLIntrospectionTypeRef<GraphQLIntrospectionTypeRef<GraphQLIntrospectionTypeRef<GraphQLIntrospectionTypeRef<GraphQLIntrospectionTypeRef<GraphQLIntrospectionOfType>>>>>>
                        {
                            Kind = GraphQLTypeKind.Object,
                            Name = Implicit_Schema_Type
                        }
                    }
                });

                // Add type
                fields.Add(new GraphQLIntrospectionField
                {
                    Name = Implicit_Type_Name,
                    Type = new GraphQLIntrospectionTypeRef
                    {
                        Kind = GraphQLTypeKind.Object,
                        Name = Implicit_Type_Type
                    },
                    Args = new List<GraphQLIntrospectionInputValue>
                    {
                        new GraphQLIntrospectionInputValue
                        {
                            Name = Implicit_Type_Argument_Name,
                            Type = new GraphQLIntrospectionTypeRef
                            {
                                Kind = GraphQLTypeKind.NonNull,
                                OfType = new GraphQLIntrospectionTypeRef<GraphQLIntrospectionTypeRef<GraphQLIntrospectionTypeRef<GraphQLIntrospectionTypeRef<GraphQLIntrospectionTypeRef<GraphQLIntrospectionTypeRef<GraphQLIntrospectionOfType>>>>>>
                                {
                                    Kind = GraphQLTypeKind.Scalar,
                                    Name = Implicit_Type_Argument_Type
                                }
                            }
                        }
                    }
                });
            }

            // Add typename
            fields.Add(new GraphQLIntrospectionField
            {
                Name = Implicit_TypeName_Name,
                Type = new GraphQLIntrospectionTypeRef
                {
                    Kind = GraphQLTypeKind.Scalar,
                    Name = Implicit_TypeName_Type
                }
            });

            return new GraphQLIntrospectionFullType
            {
                Description = graphQLType.Description,
                EnumValues = graphQLType.EnumValues,
                Fields = fields.AsEnumerable(),
                InputFields = graphQLType.InputFields,
                Interfaces = graphQLType.Interfaces,
                Kind = graphQLType.Kind,
                Name = graphQLType.Name,
                PossibleTypes = graphQLType.PossibleTypes
            };
        }
    }
}
