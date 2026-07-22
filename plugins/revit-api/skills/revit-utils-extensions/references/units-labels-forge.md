# Units, labels, and ForgeTypeId

Unit conversion and formatting, user-visible labels, and ForgeTypeId inspection.
Each `## Heading (RawClass)` names the raw Revit static this domain replaces; call the extension on the receiver shown instead.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

Revit stores lengths in feet and angles in radians. Convert at the boundary where a value enters or leaves the model.

## Unit conversion (UnitUtils)

- `69d.FromMillimeters()`;
- `value.ToMillimeters()`;
- `69d.FromMeters()`;
- `value.ToMeters()`;
- `69d.FromInches()`;
- `value.ToInches()`;
- `69d.FromDegrees()`;
- `value.ToDegrees()`;
- `69d.FromUnit(UnitTypeId.Celsius)`;
- `value.ToUnit(UnitTypeId.Celsius)`;

## Formatting (FormatUtils, Units.Format)

Both extensions are members of `Units` — get it with `document.GetUnits()`.

- `document.GetUnits().Format(SpecTypeId.Length, 69, forEditing: false)`;
- `document.GetUnits().TryParse(SpecTypeId.Length, "21031 mm", out var value)`;

`Format` accepts `FormatValueOptions`, for example `new FormatValueOptions { AppendUnitSymbol = true }`.

## Labels (LabelUtils)

`ToLabel` accepts most enums and type identifiers (`BuiltInCategory`, `BuiltInParameter`, `BuiltInParameterGroup`, `SpecTypeId`, `UnitTypeId`, `ParameterType`, `FailureSeverity`,
`StructuralSectionShape`, …) and returns the user-visible name. It takes an optional `LanguageType`.

- `BuiltInParameter.WALL_TOP_OFFSET.ToLabel()`;
- `BuiltInParameter.WALL_TOP_OFFSET.ToLabel(LanguageType.Russian)`;
- `DisciplineTypeId.Hvac.ToDisciplineLabel()`;
- `GroupTypeId.Geometry.ToGroupLabel()`;
- `ParameterTypeId.DoorCost.ToParameterLabel()`;
- `SpecTypeId.SheetLength.ToSpecLabel()`;
- `SymbolTypeId.Hour.ToSymbolLabel()`;
- `UnitTypeId.Hertz.ToUnitLabel()`;

## ForgeTypeId predicates (ParameterUtils, SpecUtils, UnitUtils)

- `forgeTypeId.IsBuiltInParameter`;
- `forgeTypeId.IsBuiltInGroup`;
- `forgeTypeId.IsSpec`;
- `symbolTypeId.IsSymbol`;
- `unitTypeId.IsUnit`;
- `forgeTypeId.IsValidDataType`;
- `specTypeId.IsMeasurableSpec`;
- `specTypeId.IsValidUnit(unitTypeId)`;

## ForgeTypeId conversions and queries

- `forgeTypeId.GetBuiltInParameter()`;
- `builtInParameter.GetParameterTypeId()`;
- `forgeTypeId.GetBuiltInParameterGroupTypeId()`;
- `specTypeId.GetDiscipline()`;
- `specTypeId.GetValidUnits()`;
- `specTypeId.GetTypeCatalogStringForSpec()`;
- `unitTypeId.GetTypeCatalogStringForUnit()`;
- `ForgeTypeId.GetAllBuiltInParameters()`;
- `ForgeTypeId.GetAllBuiltInGroups()`;
- `ForgeTypeId.GetAllSpecs()`;
- `ForgeTypeId.GetAllMeasurableSpecs()`;
- `ForgeTypeId.GetAllDisciplines()`;
- `ForgeTypeId.GetAllUnits()`;

## Parameters Service (ForgeTypeId)

- `forgeTypeId.DownloadCompanyName(document)`;
- `forgeTypeId.DownloadParameterOptions()`;
- `forgeTypeId.DownloadParameter(document, options)`;
