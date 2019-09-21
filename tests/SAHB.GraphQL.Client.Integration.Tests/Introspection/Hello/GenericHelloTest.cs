using GraphQL.Types;
using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQL.Client.Testserver.Schemas;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Introspection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Introspection.Tests.Hello
{
    public abstract class GenericHelloTest<TQueryType, TValidQuery, TQueryWithInvalidFieldName> : GraphQLQueryTestBase<TQueryType>
        where TQueryType : ObjectGraphType, new()
    {
        public GenericHelloTest(GraphQLWebApplicationFactory<GenericQuerySchema<TQueryType>> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Validate_ValidQuery_IsValid()
        {
            // Arrange
            var graphQLClient = this.GraphQLHttpClient;

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
            var validationOutput = introspectionQuery.ValidateGraphQLType<TValidQuery>(GraphQLOperationType.Query);

            // Assert
            Assert.Empty(validationOutput);
        }

        [Theory]
        [InlineData(GraphQLOperationType.Mutation)]
        [InlineData(GraphQLOperationType.Subscription)]
        public async Task Validate_ValidQuery_Invalid_QueryType(GraphQLOperationType operationType)
        {
            // Arrange
            var graphQLClient = this.GraphQLHttpClient;

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
            var validationOutput = introspectionQuery.ValidateGraphQLType<TValidQuery>(operationType);

            // Assert
            Assert.Single(validationOutput);
            Assert.Equal(ValidationType.Operation_Type_Not_Found, validationOutput.First().ValidationType);
        }

        [Fact]
        public async Task Validate_InvalidQuery_Invalid_FieldName()
        {
            // Arrange
            var graphQLClient = this.GraphQLHttpClient;

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
            var validationOutput = introspectionQuery.ValidateGraphQLType<TQueryWithInvalidFieldName>(GraphQLOperationType.Query);

            // Assert
            Assert.Single(validationOutput);
            Assert.Equal(ValidationType.Field_Not_Found, validationOutput.First().ValidationType);
        }
    }
}
