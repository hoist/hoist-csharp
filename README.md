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

Using with PowerShell
=====================

```
#Load dll into powershell
Add-Type -Path .\Hoist.Api.dll
#Create Client
$hc = New-Object Hoist.Api.HoistClient "API KEY"
#Create Collection
$votes = $hc.GetCollection("Vote")
#Show all the ids in the coolection
$votes.ToList() | ForEach-Object {$_.Get("_id")}
#Update all in the collection
$votes.ToList() | ForEach-Object {
  $_.Set("Touched", 1)
  $votes.Update($_) }
```
