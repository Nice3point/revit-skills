# Document and storage

Document version and integrity, managers, global and project parameters, parameter filtering, and extensible storage.
Each `## Heading (RawClass)` names the raw Revit static or manager this domain replaces; call the extension on the receiver shown instead.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

## Document version and integrity (Document getters)

| Extension              | Purpose                                            | Example                                                   |
|------------------------|----------------------------------------------------|-----------------------------------------------------------|
| `Version`              | DocumentVersion of the document                    | `document.Version`                                        |
| `IsValidVersionGuid`   | Whether a GUID is valid for the document           | `document.IsValidVersionGuid(versionGuid)`                |
| `CheckAllFamilies`     | Whether all loaded families have content documents | `document.CheckAllFamilies(out var corruptFamilyIds)`     |
| `CheckAllFamiliesSlow` | Deep integrity check across content documents      | `document.CheckAllFamiliesSlow(out var corruptFamilyIds)` |

## Managers and services (manager getters)

| Extension                                   | Purpose                                                | Example                                                |
|---------------------------------------------|--------------------------------------------------------|--------------------------------------------------------|
| `GetTemporaryGraphicsManager`               | TemporaryGraphicsManager of the document               | `document.GetTemporaryGraphicsManager()`               |
| `GetAnalyticalToPhysicalAssociationManager` | AnalyticalToPhysicalAssociationManager of the document | `document.GetAnalyticalToPhysicalAssociationManager()` |
| `GetLightGroupManager`                      | LightGroupManager for the document                     | `document.GetLightGroupManager()`                      |

## Global parameters (GlobalParametersManager)

| Extension                     | Purpose                                     | Example                                                    |
|-------------------------------|---------------------------------------------|------------------------------------------------------------|
| `FindGlobalParameter`         | Global parameter by name                    | `document.FindGlobalParameter(name)`                       |
| `GetAllGlobalParameters`      | All global parameters in the document       | `document.GetAllGlobalParameters()`                        |
| `GetGlobalParametersOrdered`  | Global parameters in order                  | `document.GetGlobalParametersOrdered()`                    |
| `SortGlobalParameters`        | Sort global parameters                      | `document.SortGlobalParameters(ParametersOrder.Ascending)` |
| `MoveUpOrder`                 | Move a global parameter up in order         | `globalParameter.MoveUpOrder()`                            |
| `MoveDownOrder`               | Move a global parameter down in order       | `globalParameter.MoveDownOrder()`                          |
| `IsUniqueGlobalParameterName` | Whether a name is unique among globals      | `document.IsUniqueGlobalParameterName(name)`               |
| `IsValidGlobalParameter`      | Whether an id is a global parameter         | `parameterId.IsValidGlobalParameter(document)`             |
| `AreGlobalParametersAllowed`  | Whether globals are allowed in the document | `document.AreGlobalParametersAllowed`                      |

## Project parameters

| Extension                       | Purpose                                                | Example                                                            |
|---------------------------------|--------------------------------------------------------|--------------------------------------------------------------------|
| `GetProjectParameterNames`      | Names of bound project parameters, optionally filtered | `document.GetProjectParameterNames(ParameterBindingKind.Instance)` |
| `GetProjectParameterDefinition` | Definition of a project parameter by name              | `document.GetProjectParameterDefinition("MyParameter")`            |

`GetProjectParameterNames` also filters by `BuiltInCategory` or `SpecTypeId`, for example `document.GetProjectParameterNames(BuiltInCategory.OST_Walls)`.

## Parameter filtering (ParameterFilterUtilities)

| Extension                         | Purpose                                       | Example                                                                              |
|-----------------------------------|-----------------------------------------------|--------------------------------------------------------------------------------------|
| `GetAllFilterableCategories`      | Categories usable in a ParameterFilterElement | `ParameterFilterElement.GetAllFilterableCategories()`                                |
| `GetFilterableParametersInCommon` | Filterable parameters common to categories    | `ParameterFilterElement.GetFilterableParametersInCommon(document, categories)`       |
| `GetInapplicableParameters`       | Parameters not filterable for the categories  | `ParameterFilterElement.GetInapplicableParameters(document, categories, parameters)` |
| `IsParameterApplicable`           | Whether the element supports a parameter      | `element.IsParameterApplicable(parameterId)`                                         |
| `RemoveUnfilterableCategories`    | Drop non-filterable categories from a set     | `categories.RemoveUnfilterableCategories()`                                          |

## Shared parameter definitions

| Extension                  | Purpose                                      | Example                                              |
|----------------------------|----------------------------------------------|------------------------------------------------------|
| `SearchExternalDefinition` | ExternalDefinition for a parameter in a file | `definitionFile.SearchExternalDefinition(parameter)` |

## Extensible storage (Entity / Schema)

| Extension    | Purpose                                         | Example                                             |
|--------------|-------------------------------------------------|-----------------------------------------------------|
| `SaveEntity` | Store data on the element, overwriting existing | `element.SaveEntity(schema, "data", "schemaField")` |
| `LoadEntity` | Read stored data from the element               | `element.LoadEntity<string>(schema, "schemaField")` |
