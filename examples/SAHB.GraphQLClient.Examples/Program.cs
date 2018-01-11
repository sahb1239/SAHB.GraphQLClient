using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.Extentions;
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
                            .Alias("Hero")
                            .Field("name", name => name.Alias("Name"))
                            .Field("friends", 
                                friends => 
                                    friends.Alias("Friends").Field("name", name => name.Alias("Name")))),
                "https://mpjk0plp9.lp.gql.zone/graphql");
            var builderResponse = await query.Execute();
            Console.WriteLine(builderResponse["Hero"]["Name"].Value);

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

            // Create batch
            var batch = client.CreateBatch("https://mpjk0plp9.lp.gql.zone/graphql");

            // Create two requests in the batch
            var queryId1000 = batch.Query<HumanQuery>(new GraphQLQueryArgument("humanID", "1000"));
            var queryId1001 = batch.Query<HumanQuery>(new GraphQLQueryArgument("humanID", "1001"));

            // Execute the batch
            var queryId1000Result = await queryId1000.Execute();
            var queryId1001Result = await queryId1001.Execute();

            // Get result
            Console.WriteLine(queryId1000Result.Human.Name);
            Console.WriteLine(queryId1001Result.Human.Name);

            // Create executor
            IGraphQLHttpExecutor executor = new GraphQLHttpExecutor();
            var result = await executor.ExecuteQuery<HeroQuery>(@"{""query"":""query{Hero:hero{Name:name Friends:friends{Name:name}}}""}",
                "https://mpjk0plp9.lp.gql.zone/graphql", HttpMethod.Post);
            Console.WriteLine(result.Data.Hero.Name);

            // Using dependency injection and concole logging
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

            Console.ReadKey();
        }

        public class HeroQuery
        {
            public CharacterOrPerson Hero { get; set; }
        }

        public class HumanQuery
        {
            [GraphQLArguments("id", "ID!", "humanID")]
            public CharacterOrPerson Human { get; set; }
        }

        public class CharacterOrPerson
        {
            public string Name { get; set; }
            public IEnumerable<Friend> Friends { get; set; }
        }

        public class Friend
        {
            public string Name { get; set; }
        }
    }
}
