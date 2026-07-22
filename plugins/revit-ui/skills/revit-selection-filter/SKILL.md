---
name: revit-selection-filter
description: >
  Filter which elements and references a user can pick in Autodesk Revit UI with the Nice3point.Revit.Toolkit SelectionConfiguration, not a hand-rolled ISelectionFilter.
  USE FOR: constraining an interactive Selection.PickObject/PickObjects with a fluent Allow.Element / Allow.Reference filter.
  DO NOT USE FOR: headless API callbacks such as family load or duplicate-type options (use Toolkit option handlers like FamilyLoadOptions).
license: MIT
---

# Revit Selection Filter

`Selection.PickObject` takes an `ISelectionFilter` that decides what the user may pick.
`SelectionConfiguration` (from `Nice3point.Revit.Toolkit`) builds that filter from two lambdas; you do not hand-roll a class.
It drives an interactive pick and needs the Revit UI.

## When to use

- Restricting an interactive pick to a category, type, or geometry kind.

## When not to use

- Constraining a headless API callback such as family load or duplicate-type options — use a Toolkit option handler like `FamilyLoadOptions`.

## Workflow

### Step 1: Configure the allowed elements and references

`Allow.Element` gates which elements are pickable; `Allow.Reference` gates which references (faces, edges, points).
Set either or both — each returns the configuration and they chain — then pass `configuration.Filter` to the pick call.

```csharp
var configuration = new SelectionConfiguration()
    .Allow.Element(element => element.Category.Id.IsCategory(BuiltInCategory.OST_Walls))
    .Allow.Reference((reference, position) => false);

var picked = uiDocument.Selection.PickObject(ObjectType.Element, configuration.Filter);
```

### Step 2: Verify

Run the command and confirm only the intended elements or references highlight and accept a pick.

## Validation

- [ ] The filter uses `SelectionConfiguration`, not a hand-rolled `ISelectionFilter`.
- [ ] `Allow.Element`/`Allow.Reference` express the rule with a lambda, not a new class.

## Common Pitfalls

| Pitfall                                           | Correct approach                                          |
|---------------------------------------------------|-----------------------------------------------------------|
| A bespoke `ISelectionFilter` for a category check | Use `SelectionConfiguration().Allow.Element(...)`.        |
| Filtering references with an element-only filter  | Add `Allow.Reference((reference, position) => ...)`.      |
| `SelectionConfiguration` not found                | The `Nice3point.Revit.Toolkit` package is not referenced. |
