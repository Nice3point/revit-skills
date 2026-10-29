---
name: revit-element-and-parameter-access
description: >
  Read and write Autodesk Revit elements and parameters with the Nice3point.Revit.Extensions fluent accessors.
  USE FOR: finding an Element by ElementId, finding a parameter by BuiltInParameter/ParameterTypeId/Guid/name, reading its typed value, setting it, and converting internal units.
  DO NOT USE FOR: querying the model for a set of elements (use revit-element-collector).
license: MIT
---

# Revit Element and Parameter Access

The `Nice3point.Revit.Extensions` accessors wrap the raw element and parameter API — `document.GetElement(id) as T`, the different raw parameter getters, manual `StorageType` switching, and `UnitUtils` — into one fluent, nullable chain.
`FindParameter` also falls back to the element type when the instance lacks the parameter.

## When to use

- Finding an `Element` by `ElementId`.
- Reading or writing a parameter value, or converting a stored length to display units.

## When not to use

- Finding a set of elements from the model — use `revit-element-collector`.

## Workflow

### Step 1: Get a typed element from an id

```csharp
var wall = wallId.ToElement<Wall>(document);
```

`ToElement<T>` returns null when the id references no element, and throws `InvalidCastException` when the element is of another type.
It applies to an id whose element type is known.

An id from outside the code, such as a user selection or an id another process passes, may reference an element of any type.
`ToElementOrDefault<T>` returns null for an element of another type and for an id of no element:

```csharp
var view = elementId.ToElementOrDefault<View>(document);
if (view is null)
{
    return;
}
```

### Step 2: Find a parameter

```csharp
var parameter = wall.FindParameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
```

`FindParameter` accepts a `BuiltInParameter`, `ParameterTypeId`, `Guid`, or name, and returns the type's parameter when the instance has none.
It returns null when the parameter is absent — null-check for optional parameters.

### Step 3: Read the typed value

```csharp
double heightFeet = parameter.AsDouble(); // native api call
bool isStructural = wall.FindParameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsBool();
Color color = door.FindParameter("Door color").AsColor();
Level level = wall.FindParameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElement<Level>();
Material? material = door.FindParameter("Door material").AsElementOrDefault<Material>();
```

`AsElement<T>` throws `InvalidCastException` for an element of another type, and `AsElementOrDefault<T>` returns null for it.

### Step 4: Convert internal units at the boundary

Revit stores lengths in feet.
Convert a value when it is read from or written to the model.

```csharp
double heightMm = parameter.AsDouble().ToMillimeters();
```

### Step 5: Write inside a transaction

```csharp
parameter.Set(3.0.FromMeters());
door.FindParameter("Door color").Set(new Color(66, 69, 96));
```

Writes require an open transaction.

## Validation

- [ ] An `ElementId` of a known element type is resolved with `ToElement<T>`.
- [ ] An `ElementId` that may reference an element of another type is resolved with `ToElementOrDefault<T>`, not `GetElement(id) as T`.
- [ ] Parameters are found with `FindParameter`, and optional ones are null-checked.
- [ ] Stored lengths are converted at the boundary with the unit conversion extensions.
- [ ] Writes happen inside a transaction.

## Common Pitfalls

| Pitfall                                                         | Correct approach                                             |
|-----------------------------------------------------------------|--------------------------------------------------------------|
| `document.GetElement(id) as Wall`                               | `id.ToElementOrDefault<Wall>(document)`.                     |
| `ToElement<T>` for an id that may reference another type        | `ToElementOrDefault<T>`; `ToElement<T>` throws for it.       |
| Choosing among `get_Parameter`/`LookupParameter`/`GetParameter` | `FindParameter(...)` searches the instance and its type.     |
| Using a raw feet value as millimeters                           | Convert with `.ToMillimeters()` / `.FromMeters()`.           |
| `ToElement`/`FindParameter` not found                           | The `Nice3point.Revit.Extensions` package is not referenced. |
