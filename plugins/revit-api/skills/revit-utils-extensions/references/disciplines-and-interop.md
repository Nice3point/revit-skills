# Disciplines and interoperability

MEP, structure, and analytical helpers, with model paths, worksharing, coordination models, export, external references, and DirectContext3D.
Each `## Heading (RawClass)` names the raw Revit static this domain replaces; call the extension on the receiver shown instead.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

Get/set pairs are shown on one row; call the `Get…`/`Set…` member you need. Many element members also have an `ElementId` overload taking `document`.

## MEP — Pipe (PlumbingUtils)

- `first.ConnectPipePlaceholdersAtElbow(second)`;
- `first.ConnectPipePlaceholdersAtTee(second, third)`;
- `first.ConnectPipePlaceholdersAtCross(second, third, fourth)`;
- `placeholderIds.ConvertPipePlaceholders(document)`;
- `pipe.PlaceCapOnOpenEnds()`;
- `pipe.HasOpenConnector`;
- `pipe.BreakCurve(new XYZ(1, 1, 1))`;

## MEP — Duct (MechanicalUtils)

- `duct.BreakCurve(new XYZ(1, 1, 1))`;
- `duct.ConnectAirTerminal(airTerminalId)`;
- `placeholderIds.ConvertDuctPlaceholders(document)`;
- `document.NewDuctworkStiffener(familySymbol, host, distanceFromHostEnd)`;

## MEP — Connector (MEPSupportUtils)

- `connector.ConnectDuctPlaceholdersAtElbow(other)`;
- `connector.ConnectDuctPlaceholdersAtTee(second, third)`;
- `connector.ConnectDuctPlaceholdersAtCross(second, third, fourth)`;
- `connector.ValidateFabricationConnectivity(other)`;

## MEP — Fabrication (FabricationUtils)

- `document.ExportToPcf(filename, ids)`;

## Structure — Rebar (RebarSpliceUtils, RebarShapeParameters)

- `rebar.CanBeSpliced(spliceOptions, line, linePlaneNormal)`;
- `rebar.Splice(spliceOptions, line, linePlaneNormal)`;
- `rebar.UnifyRebarsIntoOne(secondRebarId)`;
- `rebar.GetSpliceChain()`;
- `rebar.GetLapDirectionForSpliceGeometryAndPosition(spliceGeometry, splicePosition)`;
- `rebar.GetSpliceGeometries(spliceOptions, spliceRules)`;
- `sourceRebars.AlignByFace(document, sourceFaceReference, destinationFaceReference)`;
- `sourceRebars.AlignByHost(document, destinationHost)`;
- `rebarShape.GetAllParameters()`;
- `externalDefinition.IsValidRebarShapeParameter`;
- `externalDefinition.GetRebarShapeParameterElementId(document)`;
- `externalDefinition.GetOrCreateRebarShapeParameterElementId(document)`;
- `document.NewRebarSpliceType(typeName)`;
- `document.NewRebarCrankType(typeName)`;
- `element.GetRebarSpliceLapLengthMultiplier()`;
- `element.GetRebarSpliceShiftOption()`;
- `element.GetRebarSpliceStaggerLengthMultiplier()`;
- `element.GetRebarCrankLengthMultiplier()`;
- `element.GetRebarCrankOffsetMultiplier()`;
- `element.GetRebarCrankRatio()`;

## Structure — Framing (StructuralFramingUtils)

- `familyInstance.CanFlipFramingEnds`;
- `familyInstance.FlipFramingEnds()`;
- `familyInstance.IsFramingJoinAllowedAtEnd(end: 0)`;
- `familyInstance.AllowFramingJoinAtEnd(end: 0)`;
- `familyInstance.DisallowFramingJoinAtEnd(end: 0)`;
- `familyInstance.GetFramingEndReference(end: 0)`;
- `familyInstance.IsFramingEndReferenceValid(end: 0, pick)`;
- `familyInstance.CanSetFramingEndReference(end: 0)`;
- `familyInstance.SetFramingEndReference(end: 0, pick)`;
- `familyInstance.RemoveFramingEndReference(end: 0)`;

## Structure — Sections (StructuralSectionUtils)

- `familyInstance.GetStructuralSection()`;
- `familySymbol.SetStructuralSection(structuralSection)`;
- `familyInstance.GetStructuralElementDefinitionData(out var data)`;

## Analytical (AnalyticalToPhysicalAssociationManager)

- `element.IsAnalyticalElement`;
- `element.IsPhysicalElement`;

## Model path (ModelPathUtils)

- `modelPath.ConvertToUserVisiblePath()`;
- `modelGuid.ConvertToCloudPath(projectGuid, region)`;

## Worksharing (WorksharingUtils)

- `centralPath.CreateNewLocal(targetPath)`;
- `modelPath.GetUserWorksetInfo()`;
- `element.GetCheckoutStatus(out var owner)`;
- `element.GetWorksharingTooltipInfo()`;
- `element.GetModelUpdatesStatus()`;
- `document.RelinquishOwnership(relinquishOptions, transactOptions)`;
- `worksets.CheckoutWorksets(document)`;
- `elementIds.CheckoutElements(document)`;

## Coordination model (CoordinationModelLinkUtils)

- `element.IsCoordinationModelInstance`;
- `element.IsCoordinationModelType`;
- `document.LinkCoordinationModelFromLocalPath(filePath, linkOptions)`;
- `document.Link3DViewFromAutodeskDocs(accountId, projectId, fileId, viewName, linkOptions)`;
- `elementType.GetCoordinationModelTypeData()`;
- `elementType.ReloadCoordinationModel()`;
- `elementType.UnloadCoordinationModel()`;
- `elementType.ReloadAutodeskDocsCoordinationModelFrom(accountId, projectId, fileId, viewName)`;
- `elementType.ReloadLocalCoordinationModelFrom(filePath)`;
- `document.GetAllCoordinationModelInstanceIds()`;
- `document.GetAllCoordinationModelTypeIds()`;
- `element.GetCoordinationModelVisibilityOverride(view)`;
- `elementType.GetCoordinationModelColorOverride(view)`;
- `elementType.GetCoordinationModelTransparencyOverride(view)`;
- `elementType.ContainsCoordinationModelCategory(categoryName)`;
- `elementType.GetCoordinationModelColorOverrideForCategory(view, categoryName)`;
- `elementType.GetCoordinationModelVisibilityOverrideForCategory(view, categoryName)`;
- `element.GetAllPropertiesForReferenceInsideCoordinationModel(reference)`;
- `element.GetCategoryForReferenceInsideCoordinationModel(reference)`;
- `element.GetVisibilityOverrideForReferenceInsideCoordinationModel(view, reference)`;

## Export (ExportUtils)

- `element.ExportId`;
- `document.GbXmlId`;
- `surface.GetNurbsSurfaceData()`;

## External files (ExternalFileUtils)

- `element.IsExternalFileReference`;
- `document.GetAllExternalFileReferences()`;
- `element.GetExternalFileReference()`;

## External resources (ExternalResourceUtils, ExternalResourceReference)

- `resourceType.GetServers()`;
- `ExternalResourceReference.IsValidShortName(serverId, serverName)`;
- `document.GetAllExternalResourceReferences()`;

Server support checks are boolean properties on `externalResourceReference`:

- `externalResourceReference.ServerSupportsAssemblyCodeData`;
- `externalResourceReference.ServerSupportsRevitLinks`;
- `externalResourceReference.ServerSupportsCadLinks`;
- `externalResourceReference.ServerSupportsIfcLinks`;
- `externalResourceReference.ServerSupportsKeynotes`;

## Point clouds (PointCloudFilterUtils)

- `filter.GetFilteredOutline(box)`;

## DirectContext3D

- `category.IsADirectContext3DHandleCategory`;
- `category.GetDirectContext3DHandleInstances(document)`;
- `category.GetDirectContext3DHandleTypes(document)`;
- `element.IsADirectContext3DHandleInstance`;
- `element.IsADirectContext3DHandleType`;
