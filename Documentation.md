### Generate GraphQL query without using C# model
It's also possible to generate a GraphQL query without using a C# model. The following example shows how to generate the query from the first example for the Starwars api (without the aliases).

```csharp
// Get response from url using a generated object
var query = client.CreateQuery(builder => 
	builder.Field("hero", 
		hero => 
			hero
				.Field("name")
				.Field("friends", 
					friends => 
						friends.Field("name"))),
	"https://mpjk0plp9.lp.gql.zone/graphql");
var builderResponse = await query.Execute();
Console.WriteLine(builderResponse["hero"]["name"].Value);
```

The generated query is the following.
```
{"query":"query{hero{name friends{name}}}"}
```

To include the aliases the following code can be used.
```csharp
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
Console.WriteLine(builderResponse["Hero"]["Name"].Value);
```

The query generated is the following which is equal to the query generated in the first example:
```
{"query":"query{hero{name MyFriends:friends{name}}}"}
```

The builder supports fields, subfields, alias and arguments.

Note: If the alias and field name is case insensitive equal the alias is ignored

### Execute custom GraphQL query
If a custom GraphQL query is required to be executed it's also possible using the IGraphQLHttpExecutor. An example is shown here:

```csharp
IGraphQLHttpExecutor executor = new GraphQLHttpExecutor();
var result = await executor.ExecuteQuery<HeroQuery>(@"{""query"":""query{Hero:hero{Name:name Friends:friends{Name:name}}}""}",
	"https://mpjk0plp9.lp.gql.zone/graphql", HttpMethod.Post);
Console.WriteLine(result.Data.Hero.Name);
```

## Benchmarks
Some benchmarks has been developed to see how much impact the GraphQL client has on the performance when generating queries. Theese are located under benchmarks.