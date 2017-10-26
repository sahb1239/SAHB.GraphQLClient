using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Http;
using SAHB.GraphQLClient.QueryBuilder;
using Xunit;

namespace SAHB.GraphQLClient.Tests.GraphQLClient
{
    public class ExampleGraphQLQueryTest
    {
        [Fact(Skip = "Http call in test")]
        public async Task ExampleGraphQL()
        {
            // TODO: Use dependency injection (services.AddGraphQLClient()) (IServiceCollection)
            var client = new SAHB.GraphQLClient.GraphQLClient(new HttpClient(),
                new GraphQLQueryBuilder(new GraphQLFieldBuilder()));

            var response = await client.Query<Query>("https://mpjk0plp9.lp.gql.zone/graphql");

            Assert.Equal("R2-D2", response.Hero.Name);
        }

        public class Query
        {
            public CharacterOrPerson Hero { get; set; }
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
