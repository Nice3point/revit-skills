---
name: revit-utils-extensions
description: >
  Replace verbose Autodesk Revit *Utils static calls, static managers, and hand-written enum/id conversions with Nice3point.Revit.Extensions fluent extensions.
  USE FOR: any time you need to call a SomeUtils.Operation(…) or any other static Revit API helper, and any time you convert or format a Revit value.
  DO NOT USE FOR: querying the model for elements (use revit-element-collector), or reading and writing element parameters (use revit-element-and-parameter-access).
license: MIT
---

# Revit Utils Extensions

## When to use

- Reaching for any `SomeUtils.Operation()` static call or a verbose `*Manager` getter.
- Converting a `BuiltInParameter`/`BuiltInCategory`/`ForgeTypeId`, formatting or parsing a unit, or reading a color as hex or RGB.

## When not to use

- Finding a set of elements in the model — use `revit-element-collector`.
- Reading or writing a parameter on an element you already hold — use `revit-element-and-parameter-access`.

## Recognize and replace

When the raw call is `SomeUtils.Operation(document, element.Id, …)` or a long getter, the facade is a method on the natural receiver.

```csharp
ElementTransformUtils.MoveElement(document, element.Id, new XYZ(1, 1, 0)); // raw
element.Move(1, 1, 0); // facade

LabelUtils.GetLabelFor(BuiltInParameter.WALL_TOP_OFFSET); // raw
BuiltInParameter.WALL_TOP_OFFSET.ToLabel(); // facade

UnitUtils.ConvertToInternalUnits(69, UnitTypeId.Millimeters); // raw
69d.FromMillimeters(); // facade

SolidUtils.SplitVolumes(solid); // raw
solid.SplitVolumes(); // facade
```

Many members are conveniences with no raw equivalent — use them directly; do not hand-write the conversion:

```csharp
ElementId wallsId = BuiltInCategory.OST_Walls.ToElementId(); // enum -> id
Category category = BuiltInCategory.OST_Walls.ToCategory(document); // enum -> object
string hex = color.ToHex(); // Color -> "#RRGGBB"
```

## References

Each reference lists its domain's extensions in full — member, purpose, and a grounded example on the real receiver — with the raw `*Utils` class named in each section heading. 
Load the one that matches the task; do not guess a signature.

- [references/transforms-and-modeling.md](references/transforms-and-modeling.md) — **Load when:** moving, copying, joining, or cutting elements, or working with families, hosts, parts, assemblies, adaptive components, or masses.
- [references/geometry.md](references/geometry.md) — **Load when:** building or querying solids, bounding boxes, curves, points, tessellation, or view geometry.
- [references/units-labels-forge.md](references/units-labels-forge.md) — **Load when:** converting or formatting units, producing user-visible labels, or inspecting a `ForgeTypeId` spec, unit, or parameter.
- [references/converters-and-helpers.md](references/converters-and-helpers.md) — **Load when:** converting an enum to an id or object, reading a color representation, or reaching for numeric, string, cast, or application-capability helpers.
- [references/document-and-storage.md](references/document-and-storage.md) — **Load when:** reading the document version, getting a manager, working with global or project parameters, filtering parameters, or using extensible storage.
- [references/disciplines-and-interop.md](references/disciplines-and-interop.md) — **Load when:** working with MEP, structure, or analytical elements, or with model paths, worksharing, coordination models, export, external references, or DirectContext3D.

A newer Extensions package may expose members not shown here.
If the Utils wrapper is not found, read the package [README](https://raw.githubusercontent.com/Nice3point/RevitExtensions/refs/heads/main/README.md).

## Validation

- [ ] Raw `*Utils` static calls and `*Manager` getters are replaced by the receiver-first facade.
- [ ] Behavior is unchanged — facades add no new logic.
- [ ] Unit and label conversions use the extensions, not manual `UnitUtils`/`LabelUtils` calls.
- [ ] Converters and helpers (`ToElementId`, `ToHex`, `Round`, …) replace hand-written equivalents.

## Common Pitfalls

| Pitfall                                              | Correct approach                                                                  |
|------------------------------------------------------|-----------------------------------------------------------------------------------|
| `ElementTransformUtils.MoveElement(document, id, v)` | `element.Move(x, y, z)`.                                                          |
| `LabelUtils.GetLabelFor(parameter)`                  | `parameter.ToLabel()`.                                                            |
| Assuming a facade changes behavior                   | Facades only re-express the raw API; semantics match.                             |
| Hand-writing an id, color, or unit converter         | Use the built-in `ToElementId`/`ToHex`/`FromMillimeters` conveniences.            |
| Extension not found                                  | The `Nice3point.Revit.Extensions` package is not referenced.                      |
