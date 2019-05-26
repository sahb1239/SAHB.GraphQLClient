using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SAHB.GraphQLClient.Deserialization;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.Extentions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IGraphQLHttpClient client = GraphQLHttpClient.Default();

            // Get response from url using the HeroQuery object
            var response = await client.Query<HeroQuery>("https://mpjk0plp9.lp.gql.zone/graphql");
            Console.WriteLine(response.Hero.Name);
            
            // Get response from url using a generated object
            var query = client.CreateQuery(builder => 
                builder.Field("hero", 
                    hero => 
                        hero
                            .Field("name")
                            .Field("friends", 
                                friends => 
                                    friends.Alias("MyFriends").Field("name"))),
                "https://mpjk0plp9.lp.gql.zone/graphql");
            var builderResponse = await query.Execute();
            Console.WriteLine(builderResponse["hero"]["name"].Value);
            foreach (var friend in builderResponse["hero"]["MyFriends"])
            {
                Console.WriteLine(friend["name"].Value);
            }

            // Get response from url using a generated object without alias
            query = client.CreateQuery(builder =>
                    builder.Field("hero",
                        hero =>
                            hero
                                .Field("name")
                                .Field("friends",
                                    friends =>
                                        friends.Field("name"))),
                "https://mpjk0plp9.lp.gql.zone/graphql");
            builderResponse = await query.Execute();
            Console.WriteLine(builderResponse["hero"]["name"].Value);

            var character = await client.Query<CharacterQuery>("https://mpjk0plp9.lp.gql.zone/graphql", arguments: new GraphQLQueryArgument("characterID", "1000"));
            if (character.Character is Human human)
            {
                Console.WriteLine("Human!: " + human.Height);
            }

            // Create batch
            var batch = client.CreateBatch("https://mpjk0plp9.lp.gql.zone/graphql");

            // Create two requests in the batch
            var queryId1000 = batch.Query<CharacterQuery>(new GraphQLQueryArgument("characterID", "1000"));
            var queryId1001 = batch.Query<CharacterQuery>(new GraphQLQueryArgument("characterID", "1001"));

            // Execute the batch
            var queryId1000Result = await queryId1000.Execute();
            var queryId1001Result = await queryId1001.Execute();

            // Get result
            Console.WriteLine(queryId1000Result.Character.Name);
            Console.WriteLine(queryId1001Result.Character.Name);

            if (queryId1000Result.Character is Human human2)
            {
                Console.WriteLine("Human!: " + human2.Height);
            }
            else if (queryId1000Result.Character is Droid droid2)
            {
                Console.WriteLine("Droid!: " + droid2.PrimaryFunction);
            }

            // Create executor
            IGraphQLHttpExecutor executor = new GraphQLHttpExecutor();
            var result = await executor.ExecuteQuery(@"{""query"":""query{Hero:hero{Name:name Friends:friends{Name:name}}}""}",
                "https://mpjk0plp9.lp.gql.zone/graphql", HttpMethod.Post);
            IGraphQLDeserialization graphQLDeserialization = new GraphQLDeserilization();

            var deserilizedResult = graphQLDeserialization.DeserializeResult<HeroQuery>(result, null);

            Console.WriteLine(deserilizedResult.Data.Hero.Name);

            // Using dependency injection and console logging
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Information))
                .AddGraphQLHttpClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Get client
            client = serviceProvider.GetRequiredService<IGraphQLHttpClient>();

            // Get response from url using the HeroQuery object
            response = await client.Query<HeroQuery>("https://mpjk0plp9.lp.gql.zone/graphql");
            Console.WriteLine(response.Hero.Name);

            // Swapi
            var swapiResponse = await client.Query<SwapiQuery>("https://swapi.apis.guru/");
            foreach (var movie in swapiResponse.AllFilms.Films)
            {
                Console.WriteLine(movie.Title);
            }

            var filmResponse = await client.Query<FilmQuery>("https://swapi.apis.guru/",
                arguments: new GraphQLQueryArgument("filmIdVariable", "6"));
            Console.WriteLine(filmResponse.Film.Title);

            Console.ReadKey();
        }

        public class FilmQuery
        {
            [GraphQLArguments("filmID", "ID", "filmIdVariable")]
            public Film Film { get; set; }
        }

        public class SwapiQuery
        {
            public FilmConnection AllFilms { get; set; }
        }

        public class FilmConnection : Connection
        {
            public IEnumerable<Film> Films { get; set; }
        }

        public class Film
        {
            public string Title { get; set; }
        }

        public abstract class Connection
        {
            public PageInfo PageInfo { get; set; }
            public int TotalCount { get; set; }
        }

        public class PageInfo
        {
            public bool HasNextPage { get; set; }
        }

        public class HeroQuery
        {
            public CharacterOrPerson Hero { get; set; }
        }

        public class CharacterQuery
        {
            [GraphQLArguments("id", "ID!", "characterID")]
            public CharacterOrPerson Character { get; set; }
        }

        [GraphQLUnionOrInterface("Human", typeof(Human))]
        [GraphQLUnionOrInterface("Droid", typeof(Droid))]
        public class CharacterOrPerson
        {
            public string Name { get; set; }
            public IEnumerable<Friend> Friends { get; set; }
        }

        public class Human : CharacterOrPerson
        {
            public decimal Height { get; set; }
        }

        public class Droid : CharacterOrPerson
        {
            public string PrimaryFunction { get; set; }
        }

        public class Friend
        {
            public string Name { get; set; }
        }
    }
}
