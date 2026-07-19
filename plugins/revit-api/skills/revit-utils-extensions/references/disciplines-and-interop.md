# Disciplines and interoperability

MEP, structure, and analytical helpers, plus model paths, worksharing, coordination models, export, external references, and DirectContext3D.
Each `## Heading (RawClass)` names the raw Revit static this domain replaces; call the extension on the receiver shown instead.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

Get/set pairs are shown on one row; call the `Get…`/`Set…` member you need. Many element members also have an `ElementId` overload taking `document`.

## MEP — Pipe (PlumbingUtils)

| Extension                        | Purpose                                | Example                                                       |
|----------------------------------|----------------------------------------|---------------------------------------------------------------|
| `ConnectPipePlaceholdersAtElbow` | Connect placeholders as an elbow       | `first.ConnectPipePlaceholdersAtElbow(second)`                |
| `ConnectPipePlaceholdersAtTee`   | Connect placeholders as a tee          | `first.ConnectPipePlaceholdersAtTee(second, third)`           |
| `ConnectPipePlaceholdersAtCross` | Connect placeholders as a cross        | `first.ConnectPipePlaceholdersAtCross(second, third, fourth)` |
| `ConvertPipePlaceholders`        | Convert placeholders into pipes        | `placeholderIds.ConvertPipePlaceholders(document)`            |
| `PlaceCapOnOpenEnds`             | Cap open connectors on the pipe        | `pipe.PlaceCapOnOpenEnds()`                                   |
| `HasOpenConnector`               | Whether the pipe has an open connector | `pipe.HasOpenConnector`                                       |
| `BreakCurve`                     | Break the pipe at a position           | `pipe.BreakCurve(new XYZ(1, 1, 1))`                           |

## MEP — Duct (MechanicalUtils)

| Extension                 | Purpose                                    | Example                                                                  |
|---------------------------|--------------------------------------------|--------------------------------------------------------------------------|
| `BreakCurve`              | Break the duct at a position               | `duct.BreakCurve(new XYZ(1, 1, 1))`                                      |
| `ConnectAirTerminal`      | Connect an air terminal to the duct        | `duct.ConnectAirTerminal(airTerminalId)`                                 |
| `ConvertDuctPlaceholders` | Convert placeholders into ducts            | `placeholderIds.ConvertDuctPlaceholders(document)`                       |
| `NewDuctworkStiffener`    | Create a stiffener on fabrication ductwork | `document.NewDuctworkStiffener(familySymbol, host, distanceFromHostEnd)` |

## MEP — Connector (MEPSupportUtils)

| Extension                         | Purpose                                       | Example                                                           |
|-----------------------------------|-----------------------------------------------|-------------------------------------------------------------------|
| `ConnectDuctPlaceholdersAtElbow`  | Connect a pair as an elbow                    | `connector.ConnectDuctPlaceholdersAtElbow(other)`                 |
| `ConnectDuctPlaceholdersAtTee`    | Connect a trio as a tee                       | `connector.ConnectDuctPlaceholdersAtTee(second, third)`           |
| `ConnectDuctPlaceholdersAtCross`  | Connect a group as a cross                    | `connector.ConnectDuctPlaceholdersAtCross(second, third, fourth)` |
| `ValidateFabricationConnectivity` | Whether two connectors join without couplings | `connector.ValidateFabricationConnectivity(other)`                |

## MEP — Fabrication (FabricationUtils)

| Extension     | Purpose                         | Example                               |
|---------------|---------------------------------|---------------------------------------|
| `ExportToPcf` | Export fabrication parts to PCF | `document.ExportToPcf(filename, ids)` |

## Structure — Rebar (RebarSpliceUtils, RebarShapeParameters)

| Extension                                     | Purpose                                       | Example                                                                             |
|-----------------------------------------------|-----------------------------------------------|-------------------------------------------------------------------------------------|
| `CanBeSpliced`                                | Whether the rebar can be spliced              | `rebar.CanBeSpliced(spliceOptions, line, linePlaneNormal)`                          |
| `Splice`                                      | Splice the rebar along a line or geometry     | `rebar.Splice(spliceOptions, line, linePlaneNormal)`                                |
| `UnifyRebarsIntoOne`                          | Remove a splice, merging two rebars           | `rebar.UnifyRebarsIntoOne(secondRebarId)`                                           |
| `GetSpliceChain`                              | Rebars in the same splice chain               | `rebar.GetSpliceChain()`                                                            |
| `GetLapDirectionForSpliceGeometryAndPosition` | Lap direction for a geometry and position     | `rebar.GetLapDirectionForSpliceGeometryAndPosition(spliceGeometry, splicePosition)` |
| `GetSpliceGeometries`                         | Splice geometries satisfying the rules        | `rebar.GetSpliceGeometries(spliceOptions, spliceRules)`                             |
| `AlignByFace`                                 | Copy rebars aligned to a destination face     | `sourceRebars.AlignByFace(document, sourceFaceReference, destinationFaceReference)` |
| `AlignByHost`                                 | Copy rebars aligned like the source host      | `sourceRebars.AlignByHost(document, destinationHost)`                               |
| `GetAllParameters`                            | Shape parameters used by all rebar shapes     | `rebarShape.GetAllParameters()`                                                     |
| `IsValidRebarShapeParameter`                  | Whether a definition can be a shape parameter | `externalDefinition.IsValidRebarShapeParameter`                                     |
| `GetRebarShapeParameterElementId`             | Id of an external shape parameter             | `externalDefinition.GetRebarShapeParameterElementId(document)`                      |
| `GetOrCreateRebarShapeParameterElementId`     | Get or create the shape parameter id          | `externalDefinition.GetOrCreateRebarShapeParameterElementId(document)`              |
| `NewRebarSpliceType`                          | Create a rebar splice type                    | `document.NewRebarSpliceType(typeName)`                                             |
| `NewRebarCrankType`                           | Create a rebar crank type                     | `document.NewRebarCrankType(typeName)`                                              |
| `Get/SetRebarSpliceLapLengthMultiplier`       | Lap length multiplier of a splice type        | `element.GetRebarSpliceLapLengthMultiplier()`                                       |
| `Get/SetRebarSpliceShiftOption`               | Bar-shift option of a splice type             | `element.GetRebarSpliceShiftOption()`                                               |
| `Get/SetRebarSpliceStaggerLengthMultiplier`   | Stagger length multiplier of a splice type    | `element.GetRebarSpliceStaggerLengthMultiplier()`                                   |
| `Get/SetRebarCrankLengthMultiplier`           | Crank length multiplier of a crank type       | `element.GetRebarCrankLengthMultiplier()`                                           |
| `Get/SetRebarCrankOffsetMultiplier`           | Crank offset multiplier of a crank type       | `element.GetRebarCrankOffsetMultiplier()`                                           |
| `Get/SetRebarCrankRatio`                      | Crank ratio of a crank type                   | `element.GetRebarCrankRatio()`                                                      |

## Structure — Framing (StructuralFramingUtils)

| Extension                    | Purpose                                   | Example                                                   |
|------------------------------|-------------------------------------------|-----------------------------------------------------------|
| `CanFlipFramingEnds`         | Whether the framing ends can flip         | `familyInstance.CanFlipFramingEnds`                       |
| `FlipFramingEnds`            | Flip the framing ends                     | `familyInstance.FlipFramingEnds()`                        |
| `IsFramingJoinAllowedAtEnd`  | Whether the end may join others           | `familyInstance.IsFramingJoinAllowedAtEnd(end: 0)`        |
| `AllowFramingJoinAtEnd`      | Allow the end to join                     | `familyInstance.AllowFramingJoinAtEnd(end: 0)`            |
| `DisallowFramingJoinAtEnd`   | Prevent the end from joining              | `familyInstance.DisallowFramingJoinAtEnd(end: 0)`         |
| `GetFramingEndReference`     | Reference to a framing end                | `familyInstance.GetFramingEndReference(end: 0)`           |
| `IsFramingEndReferenceValid` | Whether a reference can be set for an end | `familyInstance.IsFramingEndReferenceValid(end: 0, pick)` |
| `CanSetFramingEndReference`  | Whether an end reference can be set       | `familyInstance.CanSetFramingEndReference(end: 0)`        |
| `SetFramingEndReference`     | Set the end reference                     | `familyInstance.SetFramingEndReference(end: 0, pick)`     |
| `RemoveFramingEndReference`  | Reset the end reference                   | `familyInstance.RemoveFramingEndReference(end: 0)`        |

## Structure — Sections (StructuralSectionUtils)

| Extension                            | Purpose                            | Example                                                           |
|--------------------------------------|------------------------------------|-------------------------------------------------------------------|
| `GetStructuralSection`               | Structural section of an element   | `familyInstance.GetStructuralSection()`                           |
| `SetStructuralSection`               | Set the structural section         | `familySymbol.SetStructuralSection(structuralSection)`            |
| `GetStructuralElementDefinitionData` | Structural element definition data | `familyInstance.GetStructuralElementDefinitionData(out var data)` |

## Analytical (AnalyticalToPhysicalAssociationManager)

| Extension             | Purpose                           | Example                       |
|-----------------------|-----------------------------------|-------------------------------|
| `IsAnalyticalElement` | Whether the element is analytical | `element.IsAnalyticalElement` |
| `IsPhysicalElement`   | Whether the element is physical   | `element.IsPhysicalElement`   |

## Model path (ModelPathUtils)

| Extension                  | Purpose                          | Example                                             |
|----------------------------|----------------------------------|-----------------------------------------------------|
| `ConvertToUserVisiblePath` | Model path → user-visible string | `modelPath.ConvertToUserVisiblePath()`              |
| `ConvertToCloudPath`       | Cloud GUIDs → cloud path         | `modelGuid.ConvertToCloudPath(projectGuid, region)` |

## Worksharing (WorksharingUtils)

| Extension                   | Purpose                                    | Example                                                            |
|-----------------------------|--------------------------------------------|--------------------------------------------------------------------|
| `CreateNewLocal`            | Copy a central model into a new local file | `centralPath.CreateNewLocal(targetPath)`                           |
| `GetUserWorksetInfo`        | User workset info without opening the file | `modelPath.GetUserWorksetInfo()`                                   |
| `GetCheckoutStatus`         | Ownership status of an element             | `element.GetCheckoutStatus(out var owner)`                         |
| `GetWorksharingTooltipInfo` | Worksharing tooltip info for an element    | `element.GetWorksharingTooltipInfo()`                              |
| `GetModelUpdatesStatus`     | Central-model status of an element         | `element.GetModelUpdatesStatus()`                                  |
| `RelinquishOwnership`       | Relinquish elements and worksets           | `document.RelinquishOwnership(relinquishOptions, transactOptions)` |
| `CheckoutWorksets`          | Take ownership of worksets                 | `worksets.CheckoutWorksets(document)`                              |
| `CheckoutElements`          | Take ownership of elements                 | `elementIds.CheckoutElements(document)`                            |

## Coordination model (CoordinationModelLinkUtils)

| Extension                                                      | Purpose                                        | Example                                                                                       |
|----------------------------------------------------------------|------------------------------------------------|-----------------------------------------------------------------------------------------------|
| `IsCoordinationModelInstance`                                  | Whether the element is a coordination instance | `element.IsCoordinationModelInstance`                                                         |
| `IsCoordinationModelType`                                      | Whether the element is a coordination type     | `element.IsCoordinationModelType`                                                             |
| `LinkCoordinationModelFromLocalPath`                           | Link a .nwc/.nwd from a local path             | `document.LinkCoordinationModelFromLocalPath(filePath, linkOptions)`                          |
| `Link3DViewFromAutodeskDocs`                                   | Link from Autodesk Docs data                   | `document.Link3DViewFromAutodeskDocs(accountId, projectId, fileId, viewName, linkOptions)`    |
| `GetCoordinationModelTypeData`                                 | Link data for a coordination type              | `elementType.GetCoordinationModelTypeData()`                                                  |
| `ReloadCoordinationModel`                                      | Reload a coordination type                     | `elementType.ReloadCoordinationModel()`                                                       |
| `UnloadCoordinationModel`                                      | Unload a coordination type                     | `elementType.UnloadCoordinationModel()`                                                       |
| `ReloadAutodeskDocsCoordinationModelFrom`                      | Reload a type from Autodesk Docs               | `elementType.ReloadAutodeskDocsCoordinationModelFrom(accountId, projectId, fileId, viewName)` |
| `ReloadLocalCoordinationModelFrom`                             | Reload a type from a local path                | `elementType.ReloadLocalCoordinationModelFrom(filePath)`                                      |
| `GetAllCoordinationModelInstanceIds`                           | All coordination instance ids                  | `document.GetAllCoordinationModelInstanceIds()`                                               |
| `GetAllCoordinationModelTypeIds`                               | All coordination type ids                      | `document.GetAllCoordinationModelTypeIds()`                                                   |
| `Get/SetCoordinationModelVisibilityOverride`                   | Visibility override for an instance or type    | `element.GetCoordinationModelVisibilityOverride(view)`                                        |
| `Get/SetCoordinationModelColorOverride`                        | Color override for a type                      | `elementType.GetCoordinationModelColorOverride(view)`                                         |
| `Get/SetCoordinationModelTransparencyOverride`                 | Transparency override for a type               | `elementType.GetCoordinationModelTransparencyOverride(view)`                                  |
| `ContainsCoordinationModelCategory`                            | Whether a category name exists in the type     | `elementType.ContainsCoordinationModelCategory(categoryName)`                                 |
| `Get/SetCoordinationModelColorOverrideForCategory`             | Color override for a category in the type      | `elementType.GetCoordinationModelColorOverrideForCategory(view, categoryName)`                |
| `Get/SetCoordinationModelVisibilityOverrideForCategory`        | Visibility override for a category in the type | `elementType.GetCoordinationModelVisibilityOverrideForCategory(view, categoryName)`           |
| `GetAllPropertiesForReferenceInsideCoordinationModel`          | Properties for an instance reference           | `element.GetAllPropertiesForReferenceInsideCoordinationModel(reference)`                      |
| `GetCategoryForReferenceInsideCoordinationModel`               | Category name for an instance reference        | `element.GetCategoryForReferenceInsideCoordinationModel(reference)`                           |
| `Get/SetVisibilityOverrideForReferenceInsideCoordinationModel` | Visibility for a reference inside the model    | `element.GetVisibilityOverrideForReferenceInsideCoordinationModel(view, reference)`           |

## Export (ExportUtils)

| Extension             | Purpose                               | Example                         |
|-----------------------|---------------------------------------|---------------------------------|
| `ExportId`            | GUID of the element in DWF/IFC export | `element.ExportId`              |
| `GbXmlId`             | GUID of the document in gbXML export  | `document.GbXmlId`              |
| `GetNurbsSurfaceData` | NURBS surface definition data         | `surface.GetNurbsSurfaceData()` |

## External files (ExternalFileUtils)

| Extension                      | Purpose                                 | Example                                   |
|--------------------------------|-----------------------------------------|-------------------------------------------|
| `IsExternalFileReference`      | Whether the element is an external file | `element.IsExternalFileReference`         |
| `GetAllExternalFileReferences` | Ids of all external file references     | `document.GetAllExternalFileReferences()` |
| `GetExternalFileReference`     | External file referencing data          | `element.GetExternalFileReference()`      |

## External resources (ExternalResourceUtils, ExternalResourceReference)

| Extension                          | Purpose                                     | Example                                                            |
|------------------------------------|---------------------------------------------|--------------------------------------------------------------------|
| `GetServers`                       | Servers supporting a resource type          | `resourceType.GetServers()`                                        |
| `IsValidShortName`                 | Whether a name is a valid server short name | `ExternalResourceReference.IsValidShortName(serverId, serverName)` |
| `GetAllExternalResourceReferences` | Ids referring to external resources         | `document.GetAllExternalResourceReferences()`                      |

Server support checks are boolean properties on `externalResourceReference`, each named `ServerSupports<Kind>`:

```csharp
externalResourceReference.ServerSupportsAssemblyCodeData;
externalResourceReference.ServerSupportsRevitLinks;
externalResourceReference.ServerSupportsCadLinks;
externalResourceReference.ServerSupportsIfcLinks;
externalResourceReference.ServerSupportsKeynotes;
```

## Point clouds (PointCloudFilterUtils)

| Extension            | Purpose                                | Example                          |
|----------------------|----------------------------------------|----------------------------------|
| `GetFilteredOutline` | Outline of a box part passing a filter | `filter.GetFilteredOutline(box)` |

## DirectContext3D

| Extension                           | Purpose                                                   | Example                                                |
|-------------------------------------|-----------------------------------------------------------|--------------------------------------------------------|
| `IsADirectContext3DHandleCategory`  | Whether the category is a DirectContext3D handle category | `category.IsADirectContext3DHandleCategory`            |
| `GetDirectContext3DHandleInstances` | Handle instances of the category                          | `category.GetDirectContext3DHandleInstances(document)` |
| `GetDirectContext3DHandleTypes`     | Handle types of the category                              | `category.GetDirectContext3DHandleTypes(document)`     |
| `IsADirectContext3DHandleInstance`  | Whether the element is a handle instance                  | `element.IsADirectContext3DHandleInstance`             |
| `IsADirectContext3DHandleType`      | Whether the element is a handle type                      | `element.IsADirectContext3DHandleType`                 |
