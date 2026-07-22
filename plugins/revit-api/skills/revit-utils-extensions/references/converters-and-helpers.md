# Converters and helpers

Enum/id/category converters, color representations, numeric and string helpers, and application capability checks.
Most members here add behavior the raw API has no single call for — use them directly; there is nothing to replace.
Where a `## Heading (RawClass)` names a raw static, prefer the extension over that call.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

Resolving an `ElementId` to a typed element (`wallId.ToElement<Wall>(document)`) belongs to `revit-element-and-parameter-access`.

## BuiltInParameter converters

| Extension                                                          | Purpose                                  |
|----------------------------------------------------------------------|-------------------------------------------|
| `BuiltInParameter.WALL_TOP_OFFSET.ToParameter(document)`            | BuiltInParameter → `Parameter` object    |
| `BuiltInParameter.WALL_TOP_OFFSET.ToElementId()`                    | BuiltInParameter → `ElementId`           |
| `parameterId.IsParameter(BuiltInParameter.WALL_BOTTOM_IS_ATTACHED)` | Whether an id matches a BuiltInParameter |

## BuiltInCategory converters

| Extension                                          | Purpose                                 |
|------------------------------------------------------|-------------------------------------------|
| `BuiltInCategory.OST_Walls.ToCategory(document)`   | BuiltInCategory → `Category` object     |
| `BuiltInCategory.OST_Walls.ToElementId()`          | BuiltInCategory → `ElementId`           |
| `categoryId.IsCategory(BuiltInCategory.OST_Walls)` | Whether an id matches a BuiltInCategory |

## Color representations

All return a representation of the `Color`; the return type matches the model named by the method.

| Extension                | Purpose                |
|---------------------------|-------------------------|
| `color.ToHex()`         | Hex string `#RRGGBB`   |
| `color.ToHexInteger()`  | Hex integer            |
| `color.ToRgb()`         | RGB representation     |
| `color.ToHsl()`         | HSL representation     |
| `color.ToHsv()`         | HSV representation     |
| `color.ToHsb()`         | HSB representation     |
| `color.ToHsi()`         | HSI representation     |
| `color.ToHwb()`         | HWB representation     |
| `color.ToCmyk()`        | CMYK representation    |
| `color.ToNCol()`        | NCol representation    |
| `color.ToCielab()`      | CIE LAB representation |
| `color.ToCieXyz()`      | CIE XYZ representation |
| `color.ToFloat()`       | Float representation   |
| `color.ToDecimal()`     | Decimal representation |

## Numeric and comparison

Precision defaults to Revit's `1e-9` tolerance; pass a value to override.

| Extension                              | Purpose                                        |
|-------------------------------------------|--------------------------------------------------|
| `6.56170000000000000001.Round()`       | Round to a precision, trimming tolerance noise |
| `value.IsAlmostEqual(6.5617)`          | Compare two numbers within a tolerance         |

## String and path

| Extension                            | Purpose                                          |
|-----------------------------------------|-----------------------------------------------------|
| `name.IsNullOrEmpty()`               | Whether the string is null or empty              |
| `name.IsNullOrWhiteSpace()`          | Whether the string is null, empty, or whitespace |
| `"C:/Folder".AppendPath("AddIn")`    | Combine path segments                            |

## Type and window

| Extension                                     | Purpose                               |
|--------------------------------------------------|------------------------------------------|
| `element.Cast<Wall>()`                        | Cast an object to a type              |
| `window.Show(uiApplication.MainWindowHandle)` | Show a modeless window owned by Revit |

## Application capabilities (OptionalFunctionalityUtils)

| Extension                                  | Purpose                                     |
|-----------------------------------------------|------------------------------------------------|
| `application.AsControlledApplication()`    | `Application` → `ControlledApplication`     |
| `uiApplication.AsControlledApplication()`  | `UIApplication` → `UIControlledApplication` |
| `application.GetAllCloudRegions()`         | Regions supported by the cloud service      |

Optional-module checks are boolean properties on `application`, each named `Is<Feature>Available`:

```csharp
application.IsIfcAvailable;
application.IsDgnExportAvailable;
application.IsDgnImportLinkAvailable;
application.IsDwfExportAvailable;
application.IsDwgExportAvailable;
application.IsDwgImportLinkAvailable;
application.IsDxfExportAvailable;
application.IsFbxExportAvailable;
application.IsGraphicsAvailable;
application.IsNavisworksExporterAvailable;
application.IsSatImportLinkAvailable;
application.IsShapeImporterAvailable;
application.IsSkpImportLinkAvailable;
application.Is3DmImportLinkAvailable;
application.IsAxmImportLinkAvailable;
application.IsObjImportLinkAvailable;
application.IsStlImportLinkAvailable;
application.IsStepImportLinkAvailable;
application.IsMaterialLibraryAvailable;
```
