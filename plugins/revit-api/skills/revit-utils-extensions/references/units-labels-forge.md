# Units, labels, and ForgeTypeId

Unit conversion and formatting, user-visible labels, and ForgeTypeId inspection.
Each `## Heading (RawClass)` names the raw Revit static this domain replaces; call the extension on the receiver shown instead.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

Revit stores lengths in feet and angles in radians. Convert at the boundary where a value enters or leaves the model.

## Unit conversion (UnitUtils)

| Extension         | Purpose                     | Example                            |
|-------------------|-----------------------------|------------------------------------|
| `FromMillimeters` | Millimeters → internal feet | `69d.FromMillimeters()`            |
| `ToMillimeters`   | Internal feet → millimeters | `value.ToMillimeters()`            |
| `FromMeters`      | Meters → internal feet      | `69d.FromMeters()`                 |
| `ToMeters`        | Internal feet → meters      | `value.ToMeters()`                 |
| `FromInches`      | Inches → internal feet      | `69d.FromInches()`                 |
| `ToInches`        | Internal feet → inches      | `value.ToInches()`                 |
| `FromDegrees`     | Degrees → internal radians  | `69d.FromDegrees()`                |
| `ToDegrees`       | Internal radians → degrees  | `value.ToDegrees()`                |
| `FromUnit`        | Any unit → internal format  | `69d.FromUnit(UnitTypeId.Celsius)` |
| `ToUnit`          | Internal format → any unit  | `value.ToUnit(UnitTypeId.Celsius)` |

## Formatting (FormatUtils, Units.Format)

Both extensions are members of `Units` — get it with `document.GetUnits()`.

| Extension  | Purpose                                  | Example                                                                      |
|------------|------------------------------------------|------------------------------------------------------------------------------|
| `Format`   | Format a number with units into a string | `document.GetUnits().Format(SpecTypeId.Length, 69, forEditing: false)`       |
| `TryParse` | Parse a formatted string into a number   | `document.GetUnits().TryParse(SpecTypeId.Length, "21031 mm", out var value)` |

`Format` accepts `FormatValueOptions`, for example `new FormatValueOptions { AppendUnitSymbol = true }`.

## Labels (LabelUtils)

`ToLabel` accepts most enums and type identifiers (`BuiltInCategory`, `BuiltInParameter`, `BuiltInParameterGroup`, `SpecTypeId`, `UnitTypeId`, `ParameterType`, `FailureSeverity`,
`StructuralSectionShape`, …) and returns the user-visible name. It takes an optional `LanguageType`.

| Extension           | Purpose                                   | Example                                                          |
|---------------------|-------------------------------------------|------------------------------------------------------------------|
| `ToLabel`           | User-visible name for an enum or type id  | `BuiltInParameter.WALL_TOP_OFFSET.ToLabel()`                     |
| `ToLabel(language)` | Localized user-visible name               | `BuiltInParameter.WALL_TOP_OFFSET.ToLabel(LanguageType.Russian)` |
| `ToDisciplineLabel` | Name for a discipline ForgeTypeId         | `DisciplineTypeId.Hvac.ToDisciplineLabel()`                      |
| `ToGroupLabel`      | Name for a parameter group ForgeTypeId    | `GroupTypeId.Geometry.ToGroupLabel()`                            |
| `ToParameterLabel`  | Name for a built-in parameter ForgeTypeId | `ParameterTypeId.DoorCost.ToParameterLabel()`                    |
| `ToSpecLabel`       | Name for a spec ForgeTypeId               | `SpecTypeId.SheetLength.ToSpecLabel()`                           |
| `ToSymbolLabel`     | Name for a symbol ForgeTypeId             | `SymbolTypeId.Hour.ToSymbolLabel()`                              |
| `ToUnitLabel`       | Name for a unit ForgeTypeId               | `UnitTypeId.Hertz.ToUnitLabel()`                                 |

## ForgeTypeId predicates (ParameterUtils, SpecUtils, UnitUtils)

| Extension            | Purpose                                       | Example                              |
|----------------------|-----------------------------------------------|--------------------------------------|
| `IsBuiltInParameter` | Whether the id is a built-in parameter        | `forgeTypeId.IsBuiltInParameter`     |
| `IsBuiltInGroup`     | Whether the id is a built-in parameter group  | `forgeTypeId.IsBuiltInGroup`         |
| `IsSpec`             | Whether the id is a spec                      | `forgeTypeId.IsSpec`                 |
| `IsSymbol`           | Whether the id is a symbol                    | `symbolTypeId.IsSymbol`              |
| `IsUnit`             | Whether the id is a unit                      | `unitTypeId.IsUnit`                  |
| `IsValidDataType`    | Whether the id is a valid parameter data type | `forgeTypeId.IsValidDataType`        |
| `IsMeasurableSpec`   | Whether the spec has units of measurement     | `specTypeId.IsMeasurableSpec`        |
| `IsValidUnit`        | Whether a unit is valid for a measurable spec | `specTypeId.IsValidUnit(unitTypeId)` |

## ForgeTypeId conversions and queries

| Extension                        | Purpose                              | Example                                        |
|----------------------------------|--------------------------------------|------------------------------------------------|
| `GetBuiltInParameter`            | BuiltInParameter for a parameter id  | `forgeTypeId.GetBuiltInParameter()`            |
| `GetParameterTypeId`             | ForgeTypeId for a BuiltInParameter   | `builtInParameter.GetParameterTypeId()`        |
| `GetBuiltInParameterGroupTypeId` | Group id for a built-in parameter id | `forgeTypeId.GetBuiltInParameterGroupTypeId()` |
| `GetDiscipline`                  | Discipline of a measurable spec      | `specTypeId.GetDiscipline()`                   |
| `GetValidUnits`                  | Valid units for a measurable spec    | `specTypeId.GetValidUnits()`                   |
| `GetTypeCatalogStringForSpec`    | Type-catalog string for a spec       | `specTypeId.GetTypeCatalogStringForSpec()`     |
| `GetTypeCatalogStringForUnit`    | Type-catalog string for a unit       | `unitTypeId.GetTypeCatalogStringForUnit()`     |
| `GetAllBuiltInParameters`        | Ids of all built-in parameters       | `ForgeTypeId.GetAllBuiltInParameters()`        |
| `GetAllBuiltInGroups`            | Ids of all built-in parameter groups | `ForgeTypeId.GetAllBuiltInGroups()`            |
| `GetAllSpecs`                    | Ids of all specs                     | `ForgeTypeId.GetAllSpecs()`                    |
| `GetAllMeasurableSpecs`          | Ids of all measurable specs          | `ForgeTypeId.GetAllMeasurableSpecs()`          |
| `GetAllDisciplines`              | Ids of all disciplines               | `ForgeTypeId.GetAllDisciplines()`              |
| `GetAllUnits`                    | Ids of all units                     | `ForgeTypeId.GetAllUnits()`                    |

## Parameters Service (ForgeTypeId)

| Extension                  | Purpose                                             | Example                                            |
|----------------------------|-----------------------------------------------------|----------------------------------------------------|
| `DownloadCompanyName`      | Owning account name for a parameter                 | `forgeTypeId.DownloadCompanyName(document)`        |
| `DownloadParameterOptions` | Settings for a parameter from the service           | `forgeTypeId.DownloadParameterOptions()`           |
| `DownloadParameter`        | Create a shared parameter from a service definition | `forgeTypeId.DownloadParameter(document, options)` |
