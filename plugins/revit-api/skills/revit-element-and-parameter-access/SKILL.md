---
name: revit-element-and-parameter-access
description: >
  Read and write Autodesk Revit elements and parameters with the Nice3point.Revit.Extensions fluent accessors instead of raw casts and get_Parameter calls.
  USE FOR: turning an ElementId into a typed element, finding a parameter by BuiltInParameter/ParameterTypeId/Guid/name, reading its typed value, setting it, and converting internal units.
  DO NOT USE FOR: querying the model for a set of elements (use revit-element-collector).
license: MIT
---

# Revit Element and Parameter Access

The `Nice3point.Revit.Extensions` accessors wrap the raw element and parameter API — `document.GetElement(id) as T`, the three different parameter getters, manual `StorageType` switching, and `UnitUtils` — into one fluent, null-friendly chain.
`FindParameter` also falls back to the element type when the instance lacks the parameter.

## When to use

- Resolving an `ElementId` to a typed element.
- Reading or writing a parameter value, or converting a stored length to display units.

## When not to use

- Selecting a set of elements from the model — use `revit-element-collector`.

## Workflow

### Step 1: Get a typed element from an id

```csharp
var wall = wallId.ToElement<Wall>(document);
```

Returns null when the element is missing or not that type.

### Step 2: Find a parameter

```csharp
var parameter = wall.FindParameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
```

`FindParameter` accepts a `BuiltInParameter`, `ParameterTypeId`, `Guid`, or name, and returns the type's parameter when the instance has none.
It returns null when the parameter is absent — null-check for optional parameters.

### Step 3: Read the typed value

```csharp
double heightFeet = parameter.AsDouble(); // native storage read
bool isStructural = wall.FindParameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsBool();
Color color = door.FindParameter("Door color").AsColor();
var level = wall.FindParameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElement<Level>();
```

### Step 4: Convert internal units at the boundary

Revit stores lengths in feet; convert when values leave or enter the model.

```csharp
double heightMm = parameter.AsDouble().ToMillimeters();
```

### Step 5: Write inside a transaction

```csharp
parameter.Set(3.0.FromMeters());
door.FindParameter("Door color").Set(new Color(66, 69, 96));
```

Writes require an open transaction — see `revit-code-style` for ownership.

## Validation

- [ ] `ElementId` is resolved with `ToElement<T>`, not `GetElement(id) as T`.
- [ ] Parameters are found with `FindParameter`, and optional ones are null-checked.
- [ ] Stored lengths are converted at the boundary with `ToMillimeters`/`FromMeters` etc.
- [ ] Writes happen inside a transaction.

## Common Pitfalls

| Pitfall                                                         | Correct approach                                             |
|-----------------------------------------------------------------|--------------------------------------------------------------|
| `document.GetElement(id) as Wall`                               | `id.ToElement<Wall>(document)`.                              |
| Choosing among `get_Parameter`/`LookupParameter`/`GetParameter` | `FindParameter(...)` handles instance and type.              |
| Using a raw feet value as millimeters                           | Convert with `.ToMillimeters()` / `.FromMeters()`.           |
| `ToElement`/`FindParameter` not found                           | The `Nice3point.Revit.Extensions` package is not referenced. |
