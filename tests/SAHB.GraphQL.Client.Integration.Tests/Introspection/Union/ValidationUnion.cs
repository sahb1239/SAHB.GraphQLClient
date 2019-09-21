using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQL.Client.Testserver.Tests.Schemas.CatOrDog;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.Introspection;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Introspection.Tests.Interface
{
    public class ValidationUnion : IClassFixture<GraphQLWebApplicationFactory<CatOrDogUnionSchema>>
    {
        private readonly GraphQLWebApplicationFactory<CatOrDogUnionSchema> _factory;

        public ValidationUnion(GraphQLWebApplicationFactory<CatOrDogUnionSchema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Validate_IsValid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
            var validationOutput = introspectionQuery.ValidateGraphQLType<TestHelloUnion>(GraphQLOperationType.Query);

            // Assert
            Assert.Empty(validationOutput);
        }

        private class TestHelloUnion
        {
            public CatOrDogType Cat { get; set; }
            public CatOrDogType Dog { get; set; }
        }

        [GraphQLUnionOrInterface("Cat", typeof(CatType))]
        [GraphQLUnionOrInterface("Dog", typeof(DogType))]
        public class CatOrDogType
        {
        }

        public class CatType : CatOrDogType
        {
            public string Cat { get; set; }
            public int Number { get; set; }
        }

        public class DogType : CatOrDogType
        {
            public string Dog { get; set; }
            public int Number { get; set; }
        }
    }
}
