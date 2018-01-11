# SAHB.GraphQLClient
Query HTTP api using GraphQL. The client recieves a model as typeparameter and then queries the GraphQL api and deserilize the result.

## Nuget
The library can be found on Nuget with the package name [SAHB.GraphQLClient](https://www.nuget.org/packages/SAHB.GraphQLClient/).

It can be installed using the following command in the Package Manager Console.

```
Install-Package SAHB.GraphQLClient
```

## Examples
An example for the Starwars API.

```csharp
// TODO: Use dependency injection (services.AddGraphQLHttpClient()) (IServiceCollection)
// Initilize GraphQLClient
IGraphQLHttpClient client = GraphQLHttpClient.Default();

// Get response from url
var response = await client.Query<Query>("https://mpjk0plp9.lp.gql.zone/graphql");

// Get name etc.
Console.WriteLine(response.Hero.Name);

// The query class used is
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
```

The following code requests the endpoint with the following query
```
{"query":"query{Hero:hero{Name:name Friends:friends{Name:name}}}"} 
```

The following using statements is required
```csharp
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.Extentions;
```

### Renaming of a field
To rename a field name use the attribute ```GraphQLFieldNameAttribute``` on the class or property which you want to remap. For example request the field Fullname on the property Name do the follwing.
```csharp
public class Friend
{
   [GraphQLFieldName("fullname")
   public string Name { get; set; }
}
```

This will generate the query:
```
{"query":"query{Hero:hero{Name:fullname"}
```

Note: For generating this you need to remember to add a extra Query class
```csharp
public class Query
{
   public Hero Hero { get; set; }
}
```

### Ignoring a field
To ignore a field use the attribute ```GraphQLFieldIgnoreAttribute``` on the class or property which you want to ignore. For example:
```csharp
public class Hero
{
   public string Name { get; set; }

   [GraphQLFieldIgnore]
   public string IgnoredField { get; set; }
}
```

Example for ignoring a class
```csharp
public class Hero
{
   public string Name { get; set; }

   public IgnoredClass IgnoredField { get; set; }
}

[GraphQLFieldIgnore]
public class IgnoredClass
{
   public string SomeProperty { get; set; }
}
```

This will generate the query:
```
{"query":"query{Hero:hero{Name:name"}
```

Note: For generating this you need to remember to add a extra Query class
```csharp
public class Query
{
   public Hero Hero { get; set; }
}
```

### Arguments
It's also possible to add arguments to queries. This can be done with the attribute ```GraphQLArgumentAttribute```. This attribute takes 3 arguments where the first is argument name used on the GraphQL server. The second is the argument type, for example String. The third argument is the varible name which should be used when the query is requested.

```csharp
public class Query
{
   [GraphQLArgumentAttribute("argumentName", "ArgumentType", "variableName")]
   public Hero Hero { get; set; }
}
```

The client is requested as shown here:
```csharp
var response = await client.Query<Query>("https://mpjk0plp9.lp.gql.zone/graphql", 
   arguments: new GraphQLQueryArgument("variableName", "valueToBeSent"});
```

This will generate the query (Hero contains here only the Name property):
```
{"query":"query($variableName:String){Hero:hero(argumentName:$variableName){Name:name}}","variables":{"variableName":"valueToBeSent"}}
```

### Merging multiple queries (batching)
The client supports merging multiple queries into one single query and returning the result for each query separate. This could reduce the number of request needed to a single server.

```csharp
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

// Class used
public class HumanQuery
{
	[GraphQLArguments("id", "ID!", "humanID")]
	public CharacterOrPerson Human { get; set; }
}
```

The following methods will generate the query:
```
{"query":"query($batch0_humanID:ID! $batch1_humanID:ID!){batch0_Human:human(id:$batch0_humanID){Name:name Friends:friends{Name:name}} batch1_Human:human(id:$batch1_humanID){Name:name Friends:friends{Name:name}}}","variables":{"batch0_humanID":"1000","batch1_humanID":"1001"}}
```

Note: when Execute is called on one result the batch does not support adding more request to it and will therefore throw if you try to add more requests to it. For example:
```csharp
// Create a requests in a batch and execute it
var batch = client.CreateBatch("https://mpjk0plp9.lp.gql.zone/graphql");
var queryId1000 = batch.Query<HumanQuery>(new GraphQLQueryArgument("humanID", "1000"));
var queryId1000Result = await queryId1000.Execute();

// Get another request
// This will throw a GraphQLBatchAlreadyExecutedException
var queryId1001 = batch.Query<HumanQuery>(new GraphQLQueryArgument("humanID", "1001"));
```

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
				.Alias("Hero")
				.Field("name", name => name.Alias("Name"))
				.Field("friends", 
					friends => 
						friends.Alias("Friends").Field("name", name => name.Alias("Name")))),
	"https://mpjk0plp9.lp.gql.zone/graphql");
var builderResponse = await query.Execute();
Console.WriteLine(builderResponse["Hero"]["Name"].Value);
```

The query generated is the following which is equal to the query generated in the first example:
```
{"query":"query{Hero:hero{Name:name Friends:friends{Name:name}}}"}
```

The builder supports fields, subfields, alias and arguments.

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

## Example project
An example project can be found in the path examples