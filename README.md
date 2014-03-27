hoist-csharp
============

Hoist C# Client Lib


Getting Started
---------------

### Untyped Documents
```C#
using Hoist.Api;
```

```C#
var client = new Hoist("apiKey");
var collection = client.GetCollection("bar");

collection.Insert(new HoistDocument("Name", "Jack"));

foreach(var document in collection.FindAll())
{
	Console.WriteLine(document["Name"]);
}
```

### Typed Documents

```C#
using Hoist.Api;
```

```C#
public class Person
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
}
```

```C#
var client = new Hoist("apiKey");
var collection = client.GetCollection<Person>("bar");

collection.Insert(new Person { Name = "Jack" });

foreach(var person in collection.FindAll())
{
	Console.WriteLine(person.Name);
}
```
