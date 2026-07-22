# Document and storage

Document version and integrity, managers, global and project parameters, parameter filtering, and extensible storage.
Each `## Heading (RawClass)` names the raw Revit static or manager this domain replaces; call the extension on the receiver shown instead.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

## Document version and integrity (Document getters)

- `document.Version`;
- `document.IsValidVersionGuid(versionGuid)`;
- `document.CheckAllFamilies(out var corruptFamilyIds)`;
- `document.CheckAllFamiliesSlow(out var corruptFamilyIds)`;

## Managers and services (manager getters)

- `document.GetTemporaryGraphicsManager()`;
- `document.GetAnalyticalToPhysicalAssociationManager()`;
- `document.GetLightGroupManager()`;

## Global parameters (GlobalParametersManager)

- `document.FindGlobalParameter(name)`;
- `document.GetAllGlobalParameters()`;
- `document.GetGlobalParametersOrdered()`;
- `document.SortGlobalParameters(ParametersOrder.Ascending)`;
- `globalParameter.MoveUpOrder()`;
- `globalParameter.MoveDownOrder()`;
- `document.IsUniqueGlobalParameterName(name)`;
- `parameterId.IsValidGlobalParameter(document)`;
- `document.AreGlobalParametersAllowed`;

## Project parameters

- `document.GetProjectParameterNames(ParameterBindingKind.Instance)`;
- `document.GetProjectParameterDefinition("MyParameter")`;

`GetProjectParameterNames` also filters by `BuiltInCategory` or `SpecTypeId`, for example `document.GetProjectParameterNames(BuiltInCategory.OST_Walls)`.

## Parameter filtering (ParameterFilterUtilities)

- `ParameterFilterElement.GetAllFilterableCategories()`;
- `ParameterFilterElement.GetFilterableParametersInCommon(document, categories)`;
- `ParameterFilterElement.GetInapplicableParameters(document, categories, parameters)`;
- `element.IsParameterApplicable(parameterId)`;
- `categories.RemoveUnfilterableCategories()`;

## Shared parameter definitions

- `definitionFile.SearchExternalDefinition(parameter)`;

## Extensible storage (Entity / Schema)

- `element.SaveEntity(schema, "data", "schemaField")`;
- `element.LoadEntity<string>(schema, "schemaField")`;
