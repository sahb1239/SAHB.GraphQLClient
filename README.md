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
IGraphQLFieldBuilder fieldBuilder = new GraphQLFieldBuilder();
IGraphQLQueryBuilder queryBuilder = new GraphQLQueryBuilder(fieldBuilder);
IGraphQLHttpExecutor executor = new GraphQLHttpExecutor();
IGraphQLHttpClient client = new GraphQLHttpClient(executor, queryBuilder);

// Get response from url
var response = await client.Query<Query>("https://mpjk0plp9.lp.gql.zone/graphql");

// Get name etc.
response.Hero.Name;

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
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryBuilder;
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
{"query":"query($variableName:String){Hero:hero(argumentName:$variableName){Name:name}}","Variables":"{\"variableName\":\"valueToBeSent\"}"}
```
