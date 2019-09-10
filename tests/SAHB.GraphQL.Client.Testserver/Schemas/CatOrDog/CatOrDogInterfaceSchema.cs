using GraphQL.Types;
using System.Collections.Generic;

namespace SAHB.GraphQL.Client.Testserver.Tests.Schemas.CatOrDog
{
    public class CatOrDogInterfaceSchema : Schema
    {
        public CatOrDogInterfaceSchema()
        {
            Query = new GraphQLQuery();
            RegisterType<CatGraphType>();
            RegisterType<DogGraphType>();
        }

        private class GraphQLQuery : ObjectGraphType
        {
            private readonly IDictionary<string, Animal> animals = new Dictionary<string, Animal>();

            public GraphQLQuery()
            {
                // Add animals
                animals.Add("cat", new Cat());
                animals.Add("dog", new Dog());

                // Add fields
                Field<AnimalGraphType>("cat", resolve: context => animals["cat"]);
                Field<AnimalGraphType>("dog", resolve: context => animals["dog"]);
                Field<ListGraphType<AnimalGraphType>>("animals", resolve: context => animals.Values);
            }
        }

        private class AnimalGraphType : InterfaceGraphType<Animal>
        {
            public AnimalGraphType()
            {
                Field<int>("number", e => e.Number);
            }
        }

        private class CatGraphType : ObjectGraphType<Cat>
        {
            public CatGraphType()
            {
                Name = "Cat";
                Field<StringGraphType>("cat", resolve: context => "cat");
                Field<int>("number", e => e.Number);
                Interface<AnimalGraphType>();
            }
        }

        private class DogGraphType : ObjectGraphType<Dog>
        {
            public DogGraphType()
            {
                Name = "Dog";
                Field<StringGraphType>("dog", resolve: context => "dog");
                Field<int>("number", e => e.Number);
                Interface<AnimalGraphType>();
            }
        }

        private class Animal
        {
            private static int number = 0;
            private readonly object locker = new object();

            public int Number
            {
                get
                {
                    lock (locker)
                    {
                        return number++;
                    }
                }
            }
        }

        private class Cat : Animal
        {

        }

        private class Dog : Animal
        {

        }
    }
}
