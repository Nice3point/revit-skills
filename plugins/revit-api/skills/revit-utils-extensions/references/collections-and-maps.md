# Collections and maps

Enumerating the Revit API's native arrays, sets, and maps with the element type carried into the sequence.
Each `## Heading (RawClass)` names the raw member this domain replaces; call the extension on the collection instead.
A member missing from the build means the installed `Nice3point.Revit.Extensions` version predates it.

## Arrays and sets (Cast&lt;T&gt;)

`EnumerateValues()` is available on every Revit array and set holding elements of a single type.

```csharp
foreach (var face in solid.Faces.EnumerateValues())
{
}

var areas = solid.Faces.EnumerateValues().Select(face => face.Area);
var names = categorySet.EnumerateValues().Select(category => category.Name);
```

A Revit collection stops its contract at the non-generic `IEnumerable`.
A `foreach` over it yields `object`, and every LINQ query opens with a cast naming the element type.

```csharp
var areas = solid.Faces.Cast<Face>().Select(face => face.Area); // raw
var areas = solid.Faces.EnumerateValues().Select(face => face.Area); // facade
```

Receivers include `CurveArray`, `CurveArrArray`, `FaceArray`, `EdgeArray`, `ElementArray`, `ReferenceArray`, `DoubleArray`, `PhaseArray`, `CategorySet`, `ParameterSet`, `ElementSet`, `ViewSet`, `ConnectorSet`, `GroupSet`, `DocumentSet`, …

## Maps (ForwardIterator)

Available on `BindingMap`, `DefinitionBindingMap`, `ParameterMap`, `CategoryNameMap`, and `Categories`:

| Extension                                | Purpose                                       |
|--------------------------------------------|-------------------------------------------------|
| `map.EnumerateEntries()`                 | Key and value of each entry as a tuple        |
| `map.EnumerateKeys()`                    | Keys alone                                    |
| `map.EnumerateValues()`                  | Values alone                                  |
| `map.TryGetValue(key, out var value)`    | Lookup reporting whether the key is present   |

```csharp
foreach (var (definition, binding) in document.ParameterBindings.EnumerateEntries())
{
}

if (element.ParametersMap.TryGetValue("Comments", out var comments))
{
}
```

A Revit map keeps the key of the current entry on its iterator and the value on `Current`, and a `foreach` reaches only the value.
Reading the keys through the raw API takes a hand-written loop that holds a native handle until it is disposed.

```csharp
var iterator = document.ParameterBindings.ForwardIterator(); // raw
while (iterator.MoveNext())
{
    var definition = iterator.Key;
    var binding = (Binding)iterator.Current;
}
```

Each enumeration opens its own native iterator and disposes it when the enumeration ends.

## Performance

These members are not wrappers over `Cast<T>()`; each one spends half the interop calls of the raw equivalent.

Measured on Revit 2027: arrays 10% faster, sets 5% faster, map keys 30% faster, map values 23% faster, and both map enumerations allocate 35% less.
Take a key or a value with `EnumerateKeys`/`EnumerateValues`; `EnumerateEntries` reads both sides and costs the pair.
