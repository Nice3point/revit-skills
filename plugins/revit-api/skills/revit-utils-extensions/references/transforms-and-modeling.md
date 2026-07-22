# Transforms and modeling

Editing and modeling extensions on elements, families, hosts, parts, assemblies, and masses.
Each `## Heading (RawClass)` names the raw Revit static this domain replaces; call the extension on the receiver shown instead.
Members with a heading and no raw class in parentheses add behavior the raw API has no single call for.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

Most element methods also have an `ElementId` overload that takes the `document` first — for example `elementId.Move(document, 1, 1, 0)` alongside `element.Move(1, 1, 0)`.

## Element transform (ElementTransformUtils)

- `element.Move(1, 1, 0)`;
- `element.Copy(new XYZ(1, 1, 0))`;
- `element.Rotate(axis, angle)`;
- `element.Mirror(plane)`;
- `element.CanBeMirrored`;
- `elementIds.MoveElements(document, new XYZ(1, 1, 1))`;
- `elementIds.RotateElements(document, axis, angle: 3.14)`;
- `elementIds.MirrorElements(document, plane, mirrorCopies: true)`;
- `elementIds.CopyElements(sourceView, destinationView)`;

## Element checks

- `element.CanBeDeleted`;

## Joins and instance void cuts (JoinGeometryUtils, InstanceVoidCutUtils)

- `first.JoinGeometry(second)`;
- `first.UnjoinGeometry(second)`;
- `first.AreElementsJoined(second)`;
- `element.GetJoinedElements()`;
- `first.SwitchJoinOrder(second)`;
- `first.IsCuttingElementInJoin(second)`;
- `element.CanBeCutWithVoid`;
- `element.GetCuttingVoidInstances()`;
- `element.AddInstanceVoidCut(cuttingInstance)`;
- `element.RemoveInstanceVoidCut(cuttingInstance)`;
- `element.IsInstanceVoidCutExists(cuttingInstance)`;

## Solid-solid cuts (SolidSolidCutUtils)

- `element.GetCuttingSolids()`;
- `element.GetSolidsBeingCut()`;
- `element.IsAllowedForSolidCut`;
- `element.IsElementFromAppropriateContext`;
- `first.CanElementCutElement(second, out var reason)`;
- `first.CutExistsBetweenElements(second, out var firstCutsSecond)`;
- `first.AddCutBetweenSolids(second)`;
- `first.RemoveCutBetweenSolids(second)`;
- `first.SplitFacesOfCuttingSolid(second, split: true)`;

## Family (FamilyUtils)

- `family.CanBeConvertedToFaceHostBased`;
- `family.ConvertToFaceHostBased()`;
- `family.CheckIntegrity()`;

## FamilySymbol

- `familySymbol.IsAdaptiveFamilySymbol`;
- `FamilySymbol.GetProfileSymbols(document, ProfileFamilyUsage.Any, oneCurveLoopOnly: true)`;

## FamilyInstance

- `familyInstance.IsVoidInstanceCuttingElement`;
- `familyInstance.GetElementsBeingCut()`;

## Form (FormUtils)

- `Form.CanBeDissolved(document, elements)`;
- `Form.DissolveForms(document, elements)`;

## HostObject (HostObjectUtils)

- `floor.GetBottomFaces()`;
- `floor.GetTopFaces()`;
- `wall.GetSideFaces(ShellLayerType.Interior)`;

## Wall (WallUtils)

- `wall.IsJoinAllowedAtEnd(end: 0)`;
- `wall.AllowJoinAtEnd(end: 0)`;
- `wall.DisallowJoinAtEnd(end: 0)`;

## Adaptive components (AdaptiveComponentInstanceUtils, AdaptiveComponentFamilyUtils)

- `family.IsAdaptiveComponentFamily`;
- `familyInstance.IsAdaptiveComponentInstance`;
- `familyInstance.HasAdaptiveFamilySymbol`;
- `familyInstance.IsAdaptiveInstanceFlipped`;
- `familyInstance.SetAdaptiveInstanceFlipped(flip: true)`;
- `familyInstance.MoveAdaptiveComponentInstance(transform, unHost: false)`;
- `family.GetNumberOfAdaptivePoints()`;
- `family.GetNumberOfAdaptivePlacementPoints()`;
- `family.GetNumberOfAdaptiveShapeHandlePoints()`;
- `familySymbol.CreateAdaptiveComponentInstance()`;
- `referencePoint.IsAdaptivePlacementPoint`;
- `referencePoint.IsAdaptivePoint`;
- `referencePoint.IsAdaptiveShapeHandlePoint`;
- `referencePoint.MakeAdaptivePoint(AdaptivePointType.Placement)`;
- `referencePoint.GetAdaptivePlacementNumber()`;
- `referencePoint.GetAdaptivePointConstraintType()`;
- `referencePoint.GetAdaptivePointOrientationType()`;
- `referencePoint.SetAdaptivePlacementNumber(placementNumber)`;
- `referencePoint.SetAdaptivePointConstraintType(constraintType)`;
- `referencePoint.SetAdaptivePointOrientationType(orientationType)`;
- `familyInstance.GetAdaptivePlacementPointElementRefIds()`;
- `familyInstance.GetAdaptivePointElementRefIds()`;
- `familyInstance.GetAdaptiveShapeHandlePointElementRefIds()`;

## Annotation

- `element.IsMultiAlignSupported`;
- `element.GetAnnotationOutlineWithoutLeaders()`;
- `element.MoveWithAnchoredLeaders(moveVector)`;

## Detail draw order

- `element.IsDetailElement(view)`;
- `element.BringForward(view)`;
- `element.BringToFront(view)`;
- `element.SendBackward(view)`;
- `element.SendToBack(view)`;
- `view.GetDrawOrderForDetails(detailIds)`;

## Parts (PartUtils)

- `element.HasAssociatedParts`;
- `element.GetAssociatedParts(includePartsWithAssociatedParts: true, includeAllChildren: true)`;
- `element.GetAssociatedPartMaker()`;
- `elementIds.CreateParts(document)`;
- `elementIds.DivideParts(document, intersectingReferenceIds, curveArray, sketchPlaneId)`;
- `elementIds.CreateMergedPart(document)`;
- `part.IsMergedPart`;
- `part.GetSplittingCurves()`;
- `part.GetSplittingElements()`;
- `elementIds.AreElementsValidForCreateParts(document)`;
- `linkElementId.IsValidForCreateParts(document)`;
- `elementIds.ArePartsValidForDivide(document)`;
- `elementIds.ArePartsValidForMerge(document)`;
- `elementIds.FindMergeableClusters(document)`;
- `part.GetChainLengthToOriginal()`;
- `part.GetMergedParts()`;
- `partMaker.GetPartMakerMethodToDivideVolumeFw()`;

## Assembly (AssemblyViewUtils)

- `sourceAssembly.AcquireViews(targetAssembly)`;
- `assemblyInstance.Create3DOrthographic()`;
- `assemblyInstance.CreateDetailSection(AssemblyDetailViewOrientation.ElevationBottom)`;
- `assemblyInstance.CreateMaterialTakeoff()`;
- `assemblyInstance.CreatePartList()`;
- `assemblyInstance.CreateSheet(titleBlockId)`;
- `assemblyInstance.CreateSingleCategorySchedule(scheduleCategoryId)`;

## Mass (MassInstanceUtils)

- `massInstance.IsMassFamilyInstance`;
- `massInstance.GetMassGrossFloorArea()`;
- `massInstance.GetMassGrossSurfaceArea()`;
- `massInstance.GetMassGrossVolume()`;
- `massInstance.GetMassLevelDataIds()`;
- `massInstance.GetMassJoinedElementIds()`;
- `massInstance.GetMassLevelIds()`;
- `massInstance.AddMassLevelData(levelId)`;
- `massInstance.RemoveMassLevelData(levelId)`;
