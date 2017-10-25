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
// TODO: Use dependency injection (services.AddGraphQLClient()) (IServiceCollection)
// Initilize GraphQLClient
var fieldBuilder = new GraphQLFieldBuilder();
var queryBuilder = new GraphQLQueryBuilder(fieldBuilder);
var httpClient = new Http.HttpClient();
var client = new SAHB.GraphQLClient.GraphQLClient(httpClient, queryBuilder);

// Get response from url
var response = await client.Get<Query>("https://mpjk0plp9.lp.gql.zone/graphql");

// Get name etc.
response.Hero.Name

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

### Renaming of a field
To rename a field name use the attribute ```GraphQLFieldNameAttribute``` on the class or property which you want to remap. For example request the field Fullname on the property Name do the follwing.
```csharp
public class Friend
{
   [GraphQLFieldNameAttribute("fullname")
   public string Name { get; set; }
}
```

This will remap the field fullname over to the property Name ```Name:fullname```.

### Ignoring a field
To ignore a field use the property ```GraphQLFieldIgnoreAttribute``` on the class or property which you want to ignore. For example:
```csharp
public class Friend
{
   [GraphQLFieldNameAttribute("fullname")
   public string Name { get; set; }

   [GraphQLFieldIgnoreAttribute]
   public string IgnoredField { get; set; }
}
```

Example for ignoring a class
```csharp
public class Friend
{
   [GraphQLFieldNameAttribute("fullname")
   public string Name { get; set; }

   public IgnoredClass IgnoredField { get; set; }
}

[GraphQLFieldIgnoreAttribute]
public class IgnoredClass
{
   public string SomeProperty { get; set; }
}
```

### Arguments
TODO
