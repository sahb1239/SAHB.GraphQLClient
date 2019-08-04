using SAHB.GraphQL.Client.Introspection.Validation;
using SAHB.GraphQL.Client.Testserver.Tests.Schemas.CatOrDog;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.Introspection;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Introspection.Tests.Interface
{
    public class ValidationInterfaceWithInterfaceType : IClassFixture<GraphQLWebApplicationFactory<CatOrDogInterfaceSchema>>
    {
        private readonly GraphQLWebApplicationFactory<CatOrDogInterfaceSchema> _factory;

        public ValidationInterfaceWithInterfaceType(GraphQLWebApplicationFactory<CatOrDogInterfaceSchema> factory)
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
            var validationOutput = introspectionQuery.ValidateGraphQLType<TestHelloInterface>(GraphQLOperationType.Query);

            // Assert
            Assert.Empty(validationOutput);
        }

        [Fact]
        public async Task Validate_Wrong_PossibleType()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var introspectionQuery = await graphQLClient.CreateQuery<GraphQLIntrospectionQuery>("http://localhost/graphql").Execute();
            var validationOutput = introspectionQuery.ValidateGraphQLType<TestHelloInterfaceWithWrongInterface>(GraphQLOperationType.Query);

            // Assert
            Assert.Single(validationOutput);
            Assert.Equal(ValidationType.PossibleType_Not_Found, validationOutput.First().ValidationType);
        }

        private class TestHelloInterface
        {
            public ICatOrDogType Cat { get; set; }
            public ICatOrDogType Dog { get; set; }
        }

        private class TestHelloInterfaceWithWrongInterface
        {
            public ICatOrDogTypeWithWrongInterface Cat { get; set; }
        }

        [GraphQLUnionOrInterface("Cat", typeof(CatType))]
        [GraphQLUnionOrInterface("Dog", typeof(DogType))]
        private interface ICatOrDogType
        {
            int Number { get; set; }
        }

        [GraphQLUnionOrInterface("Cat", typeof(CatType))]
        [GraphQLUnionOrInterface("Dog", typeof(DogType))]
        [GraphQLUnionOrInterface("Horse", typeof(HorseType))]
        private interface ICatOrDogTypeWithWrongInterface
        {
            int Number { get; set; }
        }

        private class CatType : ICatOrDogType
        {
            public string Cat { get; set; }
            public int Number { get; set; }
        }

        private class DogType : ICatOrDogType
        {
            public string Dog { get; set; }
            public int Number { get; set; }
        }

        private class HorseType : ICatOrDogType
        {
            public string Horse { get; set; }
            public int Number { get; set; }
        }
    }
}
