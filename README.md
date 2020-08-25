# SAHB.GraphQL.Client
Query an HTTP api using GraphQL. The client takes a model as a type parameter, queries the GraphQL api, and deserializes the result.

## Nuget
The library can be found on NuGet with the package name [SAHB.GraphQL.Client](https://www.nuget.org/packages/SAHB.GraphQL.Client/).
For pre release builds the NuGet package feed from [AppVeyor](https://ci.appveyor.com/nuget/sahb-graphqlclient-jry5sxi8qeq7) can be used.

Older versions (1.1 and later) can be found on [SAHB.GraphQLClient](https://www.nuget.org/packages/SAHB.GraphQLClient/).

It can be installed using the following command in the Package Manager Console.

```
Install-Package SAHB.GraphQL.Client
```

### The client supports the following frameworks:

[SAHB.GraphQL.Client](https://www.nuget.org/packages/SAHB.GraphQL.Client/):
- .NET Standard 1.2
- .NET Framework 4.5.2

[SAHB.GraphQL.Client.Subscription](https://www.nuget.org/packages/SAHB.GraphQL.Client.Subscription/):
- .NET Standard 2.0
- .NET Framework 4.5.2

## Documentation
Documentation can be found in this readme and in [Documentation.md](Documentation.md) file.

## Building
In order to build this project some additional steps is needed to generate a Version.props file. The easiest way is to execute the following commands:
```
git submodule update --init --recursive
cd Build
.\build.ps1 -Target "Build"
```

Another way is creating the file Version.props. The file should be at the root directory of the repository and contain the following:
```
<Project>
	<PropertyGroup>
		<Version>2.1.0-unstablebuild</Version>
	</PropertyGroup>
</Project>
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
```

The example uses the following query classes:
```csharp
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
{"query":"query{hero{name friends{name}}}"} 
```

The following using statements is required
```csharp
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.Extentions;
```

More examples can be found in [Examples.md](Examples.md)

### Arguments
It's possible to add arguments to queries. This can be done with the attribute ```GraphQLArgumentAttribute```. This attribute takes 3 arguments where the first is argument name used on the GraphQL server. The second is the argument type, for example String. The third argument is the varible name which should be used when the query is requested.

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
{"query":"query{hero(argumentName:\"valueToBeSent\"){name}}"}
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
{"query":"query{hero{Name:fullname"}
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
{"query":"query{hero{name}}"}
```

Note: For generating this you need to remember to add a extra Query class
```csharp
public class Query
{
   public Hero Hero { get; set; }
}
```

## Subscriptions
The GraphQLclient has a subscription client which can be found here: [SAHB.GraphQLClient.Subscription](https://www.nuget.org/packages/SAHB.GraphQL.Client.Subscription/).
Documentation can be found in [Documentation.md](Documentation.md) file.

## Introspection
The GraphQLclient has a package which contains a introspection query to inspect the GraphQL type system and a validator to validate C# queries against the introspection output. It can be found here: [SAHB.GraphQLClient.Introspection](https://www.nuget.org/packages/SAHB.GraphQL.Client.Introspection/).
Documentation can be found in [Documentation.md](Documentation.md) file.

## Example projects
Example projects can be found in the path examples
