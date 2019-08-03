# Examples
Examples for the [https://swapi.apis.guru/](https://swapi.apis.guru/).

## AllFilms
```csharp
var response = await client.Query<SwapiQuery>("https://swapi.apis.guru/");
foreach (var movie in response.AllFilms.Films)
{
    Console.WriteLine(movie.Title);
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
```

### Query
This generates the following query:
```
{"query":"query{allFilms{films{title} pageInfo{hasNextPage} totalCount}}"}
```

## Film
```csharp
var response = await client.Query<FilmQuery>("https://swapi.apis.guru/",
    arguments: new GraphQLQueryArgument("filmIdVariable", "6"));
Console.WriteLine(response.Film.Title);

public class FilmQuery
{
    [GraphQLArguments("filmID", "ID", "filmIdVariable")]
    public Film Film { get; set; }
}

public class Film
{
    public string Title { get; set; }
}
```

### Query
This generates the following query:
```
{"query":"query{film(filmID:"6"){title}}"}
```