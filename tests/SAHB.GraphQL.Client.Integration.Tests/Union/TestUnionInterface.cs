using GraphQL.Types;
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
    public class TestUnionWithInterfaceType : IClassFixture<GraphQLWebApplicationFactory<TestUnionWithInterfaceType.TestSchema>>
    {
        private readonly GraphQLWebApplicationFactory<TestSchema> _factory;

        public TestUnionWithInterfaceType(GraphQLWebApplicationFactory<TestSchema> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TestSimpleUnion()
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
            Assert.True(((CatType)result.Cat).Number != ((DogType)result.Dog).Number);
        }

        [Fact]
        public async Task TestBatchUnion()
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
                ((CatType)result1.Cat).Number,
                ((DogType)result1.Dog).Number,
                ((CatType)result2.Cat).Number,
                ((DogType)result2.Dog).Number
            };
            Assert.False(allNumbers.GroupBy(e => e).Where(e => e.Count() > 1).Any());
        }

        public class TestSchema : Schema
        {
            public TestSchema()
            {
                Query = new TestSchemaQuery();
            }
        }

        private class TestSchemaQuery : ObjectGraphType
        {
            public TestSchemaQuery()
            {
                Field<CatOrDog>("cat", resolve: context => new Cat());
                Field<CatOrDog>("dog", resolve: context => new Dog());
            }
        }

        public class CatOrDog : UnionGraphType
        {
            public CatOrDog()
            {
                Type<CatGraphType>();
                Type<DogGraphType>();
            }
        }

        static int Number = 0;
        static readonly object _locker = new object();
        static int GetNumber()
        {
            lock (_locker)
            {
                Number++;
                return Number;
            }
        }

        public class CatGraphType : ObjectGraphType<Cat>
        {
            public CatGraphType()
            {
                Name = "Cat";
                Field<StringGraphType>("cat", resolve: context => "cat");
                Field<IntGraphType>("number", resolve: context => GetNumber());
            }
        }

        public class DogGraphType : ObjectGraphType<Dog>
        {
            public DogGraphType()
            {
                Name = "Dog";
                Field<StringGraphType>("dog", resolve: context => "dog");
                Field<IntGraphType>("number", resolve: context => GetNumber());
            }
        }

        public class Cat
        {

        }

        public class Dog
        {

        }

        private class TestHelloUnion
        {
            public ICatOrDogType Cat { get; set; }
            public ICatOrDogType Dog { get; set; }
        }

        [GraphQLUnionOrInterface("Cat", typeof(CatType))]
        [GraphQLUnionOrInterface("Dog", typeof(DogType))]
        public interface ICatOrDogType
        {
        }

        public class CatType : ICatOrDogType
        {
            public string Cat { get; set; }
            public int Number { get; set; }
        }

        public class DogType : ICatOrDogType
        {
            public string Dog { get; set; }
            public int Number { get; set; }
        }
    }
}
