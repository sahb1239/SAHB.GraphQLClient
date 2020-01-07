using GraphQL.Types;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Introspection.Tests.Issues
{
    public class Issue91
    {
        public class Issue91Nullable : IClassFixture<GraphQLWebApplicationFactory<Issue91NullableSchema>>
        {
            private readonly GraphQLWebApplicationFactory<Issue91NullableSchema> _factory;

            public Issue91Nullable(GraphQLWebApplicationFactory<Issue91NullableSchema> factory)
            {
                _factory = factory;
            }

            [Fact]
            public async Task Validate_Query_IsValid()
            {
                // Arrange
                var client = _factory.CreateClient();
                var graphQLClient = GraphQLHttpClient.Default(client);

                // Act
                var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
                var validationOutput = introspectionQuery.ValidateGraphQLType<Issue91Query>(GraphQLOperationType.Query);

                // Assert
                Assert.Empty(validationOutput);
            }
        }

        public class Issue91NonNullable : IClassFixture<GraphQLWebApplicationFactory<Issue91NonNullableSchema>>
        {
            private readonly GraphQLWebApplicationFactory<Issue91NonNullableSchema> _factory;

            public Issue91NonNullable(GraphQLWebApplicationFactory<Issue91NonNullableSchema> factory)
            {
                _factory = factory;
            }

            [Fact]
            public async Task Validate_Query_IsValid()
            {
                // Arrange
                var client = _factory.CreateClient();
                var graphQLClient = GraphQLHttpClient.Default(client);

                // Act
                var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
                var validationOutput = introspectionQuery.ValidateGraphQLType<Issue91Query>(GraphQLOperationType.Query);

                // Assert
                Assert.Equal(2, validationOutput.Count());
                Assert.All(validationOutput, e => Assert.Equal(ValidationType.Field_Invalid_Type, e.ValidationType));
            }
        }
        public class Issue91Query
        {
            public decimal? lat { get; set; }
            public decimal? lng { get; set; }
        }

        public class Issue91NullableSchema : Schema
        {
            public Issue91NullableSchema()
            {
                Query = new GraphQLQuery();
            }

            private class GraphQLQuery : ObjectGraphType
            {
                public GraphQLQuery()
                {
                    Field<FloatGraphType>(
                        "lat",
                        resolve: context => 1.0m);
                    Field<FloatGraphType>(
                        "lng",
                        resolve: context => 1.1m);
                }
            }
        }

        public class Issue91NonNullableSchema : Schema
        {
            public Issue91NonNullableSchema()
            {
                Query = new GraphQLQuery();
            }

            private class GraphQLQuery : ObjectGraphType
            {
                public GraphQLQuery()
                {
                    Field<NonNullGraphType<FloatGraphType>>(
                        "lat",
                        resolve: context => 1.0m);
                    Field<NonNullGraphType<FloatGraphType>>(
                        "lng",
                        resolve: context => 1.1m);
                }
            }
        }
    }
}
