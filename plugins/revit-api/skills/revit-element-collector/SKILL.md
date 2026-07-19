---
name: revit-element-collector
description: >
  Query the Revit model with the Nice3point.Revit.Extensions fluent FilteredElementCollector wrappers, use Revit's parameter filters instead of loading elements and filtering with LINQ.
  USE FOR: writing or reviewing element queries that filter by class, category, or parameter value and return the first match, a count, or the full set.
  DO NOT USE FOR: reading or setting parameters on an element you already hold (use revit-element-and-parameter-access).
license: MIT
---

# Revit Element Collector

Query the model through the fluent `FilteredElementCollector` wrappers in `Nice3point.Revit.Extensions`.
They wrap the raw Revit collector API into a single readable chain, and keep filtering inside Revit's native database engine — which evaluates filters before elements are expanded into memory — instead of materializing every element and filtering with LINQ.

## When to use

- Retrieving elements, types, or instances by class, category, or parameter value.
- Reviewing code that writes `new FilteredElementCollector(document)` by hand or filters collector output with `.Where(...)`.
- Fetching just the first match or a count without loading the whole result set.

## When not to use

- You already hold the element and only need to read or set its parameters — use `revit-element-and-parameter-access`.
- You need a specialized `ElementFilter` the extensions do not expose — fall back to a raw `FilteredElementCollector` with that filter.

## Workflow

### Step 1: Open a collector from the document

```csharp
var collector = document.CollectElements(); // whole document
var viewScoped = document.CollectElements(view); // elements visible in a view
var restricted = document.CollectElements(elementIds); // a known id set
```

### Step 2: Apply quick filters as generic, typed calls

Prefer the generic and multi-argument overloads over raw `typeof` filters.

```csharp
var walls = document.CollectElements()
    .OfClass<Wall>()
    .ToElements();

var openings = document.CollectElements()
    .OfCategories(BuiltInCategory.OST_Walls, BuiltInCategory.OST_Floors)
    .Instances() // only instances; use .Types() for element types
    .ToElements();
```

### Step 3: Filter by parameter with native rules, not LINQ

`WhereParameter` builds a native `ElementParameterFilter` — evaluated at the database level before element expansion — then continues the collector chain.

```csharp
var tallWalls = document.CollectElements()
    .OfClass<Wall>()
    .WhereParameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).IsGreaterThan(3.0, 1e-6)
    .ToElements();
```

```csharp
// BAD — loads every wall into memory, then filters
var tallWalls = new FilteredElementCollector(document)
    .OfClass(typeof(Wall))
    .Cast<Wall>()
    .Where(wall => wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble() > 3.0);
```

Comparisons include `Equals`, `IsGreaterThan`/`IsGreaterThanOrEqualTo`, `IsLessThan`/`IsLessThanOrEqualTo` (with an optional `double` epsilon), `Contains`/`StartsWith` (and their `Not…` inversions), and `HasValue`.

### Step 4: Terminate with the fast native path

Use the extension terminators instead of LINQ — they call Revit's native fast implementations (`Count` uses `GetElementCount()`).

```csharp
var firstRoom = document.CollectElements().OfClass<SpatialElement>().FirstOrDefault();
var wallCount = document.CollectElements().OfClass<Wall>().Count();
var hasLevels = document.CollectElements().OfClass<Level>().Any();
```

### Step 5: Verify

1. The query builds the filter through collector methods, not `.Where(...)` over materialized elements.
2. A parameter comparison uses `WhereParameter`, not a post-collection predicate.
3. The terminator is a native call (`ToElements`, `First`, `FirstOrDefault`, `Count`, `Any`).

## Validation

- [ ] The collector comes from `document.CollectElements(...)`, not a hand-written `new FilteredElementCollector(...)`.
- [ ] Class and category filters use the generic/multi-argument extensions.
- [ ] Parameter conditions use `WhereParameter(...)`, keeping filtering at the database level.
- [ ] Single-result and count queries use `First`/`FirstOrDefault`/`Count`/`Any`, not LINQ.
- [ ] A quick filter (class/category) precedes a slow filter (parameter) to shrink the candidate set.

## Common Pitfalls

| Pitfall                                                                      | Correct approach                                                            |
|------------------------------------------------------------------------------|-----------------------------------------------------------------------------|
| `.Where(element => element.get_Parameter(...).AsDouble() > x)` after the collector | `WhereParameter(param).IsGreaterThan(x, epsilon)` — a native rule.          |
| `collector.ToElements().Count` / `.Any()` (LINQ over the list)               | `collector.Count()` / `collector.Any()` — native fast paths.                |
| Comparing `double` parameters without an epsilon                             | Use the `IsGreaterThan(value, epsilon)` overload.                           |
| `CollectElements` or `WhereParameter` not found                              | The `Nice3point.Revit.Extensions` package is not referenced in the project. |
| Reusing one collector after a terminator                                     | Start a fresh `CollectElements()` per query; terminators stop the iterator. |
