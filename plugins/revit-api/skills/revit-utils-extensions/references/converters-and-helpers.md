# Converters and helpers

Enum/id/category converters, color representations, numeric and string helpers, and application capability checks.
Most members here add behavior the raw API has no single call for — use them directly; there is nothing to replace.
Where a `## Heading (RawClass)` names a raw static, prefer the extension over that call.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

Resolving an `ElementId` to a typed element (`wallId.ToElement<Wall>(document)`) belongs to `revit-element-and-parameter-access`.

## BuiltInParameter converters

| Extension     | Purpose                                  | Example                                                             |
|---------------|------------------------------------------|---------------------------------------------------------------------|
| `ToParameter` | BuiltInParameter → `Parameter` object    | `BuiltInParameter.WALL_TOP_OFFSET.ToParameter(document)`            |
| `ToElementId` | BuiltInParameter → `ElementId`           | `BuiltInParameter.WALL_TOP_OFFSET.ToElementId()`                    |
| `IsParameter` | Whether an id matches a BuiltInParameter | `parameterId.IsParameter(BuiltInParameter.WALL_BOTTOM_IS_ATTACHED)` |

## BuiltInCategory converters

| Extension     | Purpose                                 | Example                                            |
|---------------|-----------------------------------------|----------------------------------------------------|
| `ToCategory`  | BuiltInCategory → `Category` object     | `BuiltInCategory.OST_Walls.ToCategory(document)`   |
| `ToElementId` | BuiltInCategory → `ElementId`           | `BuiltInCategory.OST_Walls.ToElementId()`          |
| `IsCategory`  | Whether an id matches a BuiltInCategory | `categoryId.IsCategory(BuiltInCategory.OST_Walls)` |

## Color representations

All return a representation of the `Color`; the return type matches the model named by the method.

| Extension      | Purpose                | Example                |
|----------------|------------------------|------------------------|
| `ToHex`        | Hex string `#RRGGBB`   | `color.ToHex()`        |
| `ToHexInteger` | Hex integer            | `color.ToHexInteger()` |
| `ToRgb`        | RGB representation     | `color.ToRgb()`        |
| `ToHsl`        | HSL representation     | `color.ToHsl()`        |
| `ToHsv`        | HSV representation     | `color.ToHsv()`        |
| `ToHsb`        | HSB representation     | `color.ToHsb()`        |
| `ToHsi`        | HSI representation     | `color.ToHsi()`        |
| `ToHwb`        | HWB representation     | `color.ToHwb()`        |
| `ToCmyk`       | CMYK representation    | `color.ToCmyk()`       |
| `ToNCol`       | NCol representation    | `color.ToNCol()`       |
| `ToCielab`     | CIE LAB representation | `color.ToCielab()`     |
| `ToCieXyz`     | CIE XYZ representation | `color.ToCieXyz()`     |
| `ToFloat`      | Float representation   | `color.ToFloat()`      |
| `ToDecimal`    | Decimal representation | `color.ToDecimal()`    |

## Numeric and comparison

Precision defaults to Revit's `1e-9` tolerance; pass a value to override.

| Extension       | Purpose                                        | Example                          |
|-----------------|------------------------------------------------|----------------------------------|
| `Round`         | Round to a precision, trimming tolerance noise | `6.56170000000000000001.Round()` |
| `IsAlmostEqual` | Compare two numbers within a tolerance         | `value.IsAlmostEqual(6.5617)`    |

## String and path

| Extension            | Purpose                                          | Example                           |
|----------------------|--------------------------------------------------|-----------------------------------|
| `IsNullOrEmpty`      | Whether the string is null or empty              | `name.IsNullOrEmpty()`            |
| `IsNullOrWhiteSpace` | Whether the string is null, empty, or whitespace | `name.IsNullOrWhiteSpace()`       |
| `AppendPath`         | Combine path segments                            | `"C:/Folder".AppendPath("AddIn")` |

## Type and window

| Extension | Purpose                               | Example                                       |
|-----------|---------------------------------------|-----------------------------------------------|
| `Cast<T>` | Cast an object to a type              | `element.Cast<Wall>()`                        |
| `Show`    | Show a modeless window owned by Revit | `window.Show(uiApplication.MainWindowHandle)` |

## Application capabilities (OptionalFunctionalityUtils)

| Extension                 | Purpose                                     | Example                                   |
|---------------------------|---------------------------------------------|-------------------------------------------|
| `AsControlledApplication` | `Application` → `ControlledApplication`     | `application.AsControlledApplication()`   |
| `AsControlledApplication` | `UIApplication` → `UIControlledApplication` | `uiApplication.AsControlledApplication()` |
| `GetAllCloudRegions`      | Regions supported by the cloud service      | `application.GetAllCloudRegions()`        |

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
