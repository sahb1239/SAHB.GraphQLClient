using SAHB.GraphQL.Client.Testserver.Tests.Schemas.CatOrDog;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SAHB.GraphQL.Client.Integration.Tests
{
    public class TestInterface : IClassFixture<GraphQLWebApplicationFactory<CatOrDogInterfaceSchema>>
    {
        private readonly GraphQLWebApplicationFactory<CatOrDogInterfaceSchema> _factory;

        public TestInterface(GraphQLWebApplicationFactory<CatOrDogInterfaceSchema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestSimpleInterface()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var result = await graphQLClient.Execute<TestHelloUnion>(GraphQLOperationType.Query, "http://localhost/graphql");

            // Assert
            Assert.Equal(typeof(CatType), result.Cat.GetType());
            Assert.Equal(typeof(DogType), result.Dog.GetType());
            Assert.Equal("cat", ((CatType)result.Cat).Cat);
            Assert.Equal("dog", ((DogType)result.Dog).Dog);

            // Test number is different
            Assert.True(result.Cat.Number != result.Dog.Number);
        }

        [Fact]
        public async Task TestBatchInterface()
        {
            // Arrange
            var client = _factory.CreateClient();
            var graphQLClient = GraphQLHttpClient.Default(client);

            // Act
            var batch = graphQLClient.CreateBatch(GraphQLOperationType.Query, "http://localhost/graphql");
            var query1 = batch.Query<TestHelloUnion>();
            var query2 = batch.Query<TestHelloUnion>();

            var result1 = await query1.Execute();
            var result2 = await query2.Execute();

            // Assert
            Assert.Equal(typeof(CatType), result1.Cat.GetType());
            Assert.Equal(typeof(DogType), result1.Dog.GetType());
            Assert.Equal("cat", ((CatType)result1.Cat).Cat);
            Assert.Equal("dog", ((DogType)result1.Dog).Dog);

            Assert.Equal(typeof(CatType), result2.Cat.GetType());
            Assert.Equal(typeof(DogType), result2.Dog.GetType());
            Assert.Equal("cat", ((CatType)result2.Cat).Cat);
            Assert.Equal("dog", ((DogType)result2.Dog).Dog);

            // Test number is different
            var allNumbers = new List<int>
            {
                result1.Cat.Number,
                result1.Dog.Number,
                result2.Cat.Number,
                result2.Dog.Number
            };
            Assert.False(allNumbers.GroupBy(e => e).Where(e => e.Count() > 1).Any());
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
            public int Number { get; set; }
        }

        public class CatType : CatOrDogType
        {
            public string Cat { get; set; }
        }

        public class DogType : CatOrDogType
        {
            public string Dog { get; set; }
        }
    }
}
