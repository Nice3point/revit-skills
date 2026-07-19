# Transforms and modeling

Editing and modeling extensions on elements, families, hosts, parts, assemblies, and masses.
Each `## Heading (RawClass)` names the raw Revit static this domain replaces; call the extension on the receiver shown instead.
Members with a heading and no raw class in parentheses add behavior the raw API has no single call for.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

Most element methods also have an `ElementId` overload that takes the `document` first — for example `elementId.Move(document, 1, 1, 0)` alongside `element.Move(1, 1, 0)`.

## Element transform (ElementTransformUtils)

| Extension        | Purpose                                                     | Example                                                          |
|------------------|-------------------------------------------------------------|------------------------------------------------------------------|
| `Move`           | Move the element by a vector                                | `element.Move(1, 1, 0)`                                          |
| `Copy`           | Copy the element to a translated location                   | `element.Copy(new XYZ(1, 1, 0))`                                 |
| `Rotate`         | Rotate the element about an axis by an angle                | `element.Rotate(axis, angle)`                                    |
| `Mirror`         | Mirror the element across a plane                           | `element.Mirror(plane)`                                          |
| `CanBeMirrored`  | Whether the element or set can be mirrored                  | `element.CanBeMirrored`                                          |
| `MoveElements`   | Move a set of ids by a vector                               | `elementIds.MoveElements(document, new XYZ(1, 1, 1))`            |
| `RotateElements` | Rotate a set of ids about an axis                           | `elementIds.RotateElements(document, axis, angle: 3.14)`         |
| `MirrorElements` | Mirror a set of ids, optionally keeping copies              | `elementIds.MirrorElements(document, plane, mirrorCopies: true)` |
| `CopyElements`   | Copy ids within a view, between views, or between documents | `elementIds.CopyElements(sourceView, destinationView)`           |

## Element checks

| Extension      | Purpose                            | Example                |
|----------------|------------------------------------|------------------------|
| `CanBeDeleted` | Whether the element can be deleted | `element.CanBeDeleted` |

## Joins and instance void cuts (JoinGeometryUtils, InstanceVoidCutUtils)

| Extension                 | Purpose                                          | Example                                            |
|---------------------------|--------------------------------------------------|----------------------------------------------------|
| `JoinGeometry`            | Create a clean join between two elements         | `first.JoinGeometry(second)`                       |
| `UnjoinGeometry`          | Remove a join between two elements               | `first.UnjoinGeometry(second)`                     |
| `AreElementsJoined`       | Whether two elements are joined                  | `first.AreElementsJoined(second)`                  |
| `GetJoinedElements`       | All elements joined to this one                  | `element.GetJoinedElements()`                      |
| `SwitchJoinOrder`         | Reverse the join order of two elements           | `first.SwitchJoinOrder(second)`                    |
| `IsCuttingElementInJoin`  | Whether the first cuts the second in a join      | `first.IsCuttingElementInJoin(second)`             |
| `CanBeCutWithVoid`        | Whether the element accepts unattached void cuts | `element.CanBeCutWithVoid`                         |
| `GetCuttingVoidInstances` | Ids of void instances cutting the element        | `element.GetCuttingVoidInstances()`                |
| `AddInstanceVoidCut`      | Cut the element with a cutting instance's voids  | `element.AddInstanceVoidCut(cuttingInstance)`      |
| `RemoveInstanceVoidCut`   | Remove a void cut applied by an instance         | `element.RemoveInstanceVoidCut(cuttingInstance)`   |
| `IsInstanceVoidCutExists` | Whether an instance is cutting the element       | `element.IsInstanceVoidCutExists(cuttingInstance)` |

## Solid-solid cuts (SolidSolidCutUtils)

| Extension                         | Purpose                                         | Example                                                           |
|-----------------------------------|-------------------------------------------------|-------------------------------------------------------------------|
| `GetCuttingSolids`                | Solids that cut the element                     | `element.GetCuttingSolids()`                                      |
| `GetSolidsBeingCut`               | Solids the element cuts                         | `element.GetSolidsBeingCut()`                                     |
| `IsAllowedForSolidCut`            | Whether the element is eligible for a solid cut | `element.IsAllowedForSolidCut`                                    |
| `IsElementFromAppropriateContext` | Whether the element is from a valid document    | `element.IsElementFromAppropriateContext`                         |
| `CanElementCutElement`            | Whether the element can add a cut to another    | `first.CanElementCutElement(second, out var reason)`              |
| `CutExistsBetweenElements`        | Whether a solid cut links two elements          | `first.CutExistsBetweenElements(second, out var firstCutsSecond)` |
| `AddCutBetweenSolids`             | Add a solid-solid cut between two elements      | `first.AddCutBetweenSolids(second)`                               |
| `RemoveCutBetweenSolids`          | Remove the solid-solid cut                      | `first.RemoveCutBetweenSolids(second)`                            |
| `SplitFacesOfCuttingSolid`        | Split or unsplit the cutting element's faces    | `first.SplitFacesOfCuttingSolid(second, split: true)`             |

## Family (FamilyUtils)

| Extension                       | Purpose                                            | Example                                |
|---------------------------------|----------------------------------------------------|----------------------------------------|
| `CanBeConvertedToFaceHostBased` | Whether the family can become face-host-based      | `family.CanBeConvertedToFaceHostBased` |
| `ConvertToFaceHostBased`        | Convert the family to face-host-based              | `family.ConvertToFaceHostBased()`      |
| `CheckIntegrity`                | Whether the loaded family has its content document | `family.CheckIntegrity()`              |

## FamilySymbol

| Extension                | Purpose                                       | Example                                                                                    |
|--------------------------|-----------------------------------------------|--------------------------------------------------------------------------------------------|
| `IsAdaptiveFamilySymbol` | Whether the symbol is a valid adaptive symbol | `familySymbol.IsAdaptiveFamilySymbol`                                                      |
| `GetProfileSymbols`      | Profile family symbols in the document        | `FamilySymbol.GetProfileSymbols(document, ProfileFamilyUsage.Any, oneCurveLoopOnly: true)` |

## FamilyInstance

| Extension                      | Purpose                                     | Example                                       |
|--------------------------------|---------------------------------------------|-----------------------------------------------|
| `IsVoidInstanceCuttingElement` | Whether the instance's voids can cut others | `familyInstance.IsVoidInstanceCuttingElement` |
| `GetElementsBeingCut`          | Ids of elements the instance cuts           | `familyInstance.GetElementsBeingCut()`        |

## Form (FormUtils)

| Extension        | Purpose                                     | Example                                   |
|------------------|---------------------------------------------|-------------------------------------------|
| `CanBeDissolved` | Whether the forms can be dissolved          | `Form.CanBeDissolved(document, elements)` |
| `DissolveForms`  | Dissolve forms into their defining elements | `Form.DissolveForms(document, elements)`  |

## HostObject (HostObjectUtils)

| Extension        | Purpose                            | Example                                      |
|------------------|------------------------------------|----------------------------------------------|
| `GetBottomFaces` | Bottom faces of the host object    | `floor.GetBottomFaces()`                     |
| `GetTopFaces`    | Top faces of the host object       | `floor.GetTopFaces()`                        |
| `GetSideFaces`   | Major side faces for a shell layer | `wall.GetSideFaces(ShellLayerType.Interior)` |

## Wall (WallUtils)

| Extension            | Purpose                           | Example                           |
|----------------------|-----------------------------------|-----------------------------------|
| `IsJoinAllowedAtEnd` | Whether the wall end allows joins | `wall.IsJoinAllowedAtEnd(end: 0)` |
| `AllowJoinAtEnd`     | Allow the wall end to join        | `wall.AllowJoinAtEnd(end: 0)`     |
| `DisallowJoinAtEnd`  | Prevent the wall end from joining | `wall.DisallowJoinAtEnd(end: 0)`  |

## Adaptive components (AdaptiveComponentInstanceUtils, AdaptiveComponentFamilyUtils)

| Extension                                  | Purpose                                             | Example                                                                  |
|--------------------------------------------|-----------------------------------------------------|--------------------------------------------------------------------------|
| `IsAdaptiveComponentFamily`                | Whether the family is an adaptive family            | `family.IsAdaptiveComponentFamily`                                       |
| `IsAdaptiveComponentInstance`              | Whether the instance is an adaptive instance        | `familyInstance.IsAdaptiveComponentInstance`                             |
| `HasAdaptiveFamilySymbol`                  | Whether the instance has an adaptive symbol         | `familyInstance.HasAdaptiveFamilySymbol`                                 |
| `IsAdaptiveInstanceFlipped`                | Value of the instance flip parameter                | `familyInstance.IsAdaptiveInstanceFlipped`                               |
| `SetAdaptiveInstanceFlipped`               | Set the instance flip parameter                     | `familyInstance.SetAdaptiveInstanceFlipped(flip: true)`                  |
| `MoveAdaptiveComponentInstance`            | Move the adaptive instance by a transform           | `familyInstance.MoveAdaptiveComponentInstance(transform, unHost: false)` |
| `GetNumberOfAdaptivePoints`                | Count of adaptive point elements                    | `family.GetNumberOfAdaptivePoints()`                                     |
| `GetNumberOfAdaptivePlacementPoints`       | Count of placement point elements                   | `family.GetNumberOfAdaptivePlacementPoints()`                            |
| `GetNumberOfAdaptiveShapeHandlePoints`     | Count of shape-handle point elements                | `family.GetNumberOfAdaptiveShapeHandlePoints()`                          |
| `CreateAdaptiveComponentInstance`          | Create an instance of an adaptive family            | `familySymbol.CreateAdaptiveComponentInstance()`                         |
| `IsAdaptivePlacementPoint`                 | Whether the reference point is a placement point    | `referencePoint.IsAdaptivePlacementPoint`                                |
| `IsAdaptivePoint`                          | Whether the reference point is an adaptive point    | `referencePoint.IsAdaptivePoint`                                         |
| `IsAdaptiveShapeHandlePoint`               | Whether the reference point is a shape-handle point | `referencePoint.IsAdaptiveShapeHandlePoint`                              |
| `MakeAdaptivePoint`                        | Toggle a reference point's adaptive type            | `referencePoint.MakeAdaptivePoint(AdaptivePointType.Placement)`          |
| `GetAdaptivePlacementNumber`               | Placement number of a placement point               | `referencePoint.GetAdaptivePlacementNumber()`                            |
| `GetAdaptivePointConstraintType`           | Constraint type of a shape-handle point             | `referencePoint.GetAdaptivePointConstraintType()`                        |
| `GetAdaptivePointOrientationType`          | Orientation type of a placement point               | `referencePoint.GetAdaptivePointOrientationType()`                       |
| `SetAdaptivePlacementNumber`               | Set a placement point's number                      | `referencePoint.SetAdaptivePlacementNumber(placementNumber)`             |
| `SetAdaptivePointConstraintType`           | Set a shape-handle point's constraint type          | `referencePoint.SetAdaptivePointConstraintType(constraintType)`          |
| `SetAdaptivePointOrientationType`          | Set a placement point's orientation type            | `referencePoint.SetAdaptivePointOrientationType(orientationType)`        |
| `GetAdaptivePlacementPointElementRefIds`   | Placement point ref ids the instance adapts to      | `familyInstance.GetAdaptivePlacementPointElementRefIds()`                |
| `GetAdaptivePointElementRefIds`            | All adaptive point ref ids the instance adapts to   | `familyInstance.GetAdaptivePointElementRefIds()`                         |
| `GetAdaptiveShapeHandlePointElementRefIds` | Shape-handle point ref ids                          | `familyInstance.GetAdaptiveShapeHandlePointElementRefIds()`              |

## Annotation

| Extension                            | Purpose                                        | Example                                        |
|--------------------------------------|------------------------------------------------|------------------------------------------------|
| `IsMultiAlignSupported`              | Whether the element aligns to similar elements | `element.IsMultiAlignSupported`                |
| `GetAnnotationOutlineWithoutLeaders` | Four model-space corners without leaders       | `element.GetAnnotationOutlineWithoutLeaders()` |
| `MoveWithAnchoredLeaders`            | Move while keeping leader ends anchored        | `element.MoveWithAnchoredLeaders(moveVector)`  |

## Detail draw order

| Extension                | Purpose                                             | Example                                  |
|--------------------------|-----------------------------------------------------|------------------------------------------|
| `IsDetailElement`        | Whether the element participates in detail ordering | `element.IsDetailElement(view)`          |
| `BringForward`           | Move the detail one step toward the front           | `element.BringForward(view)`             |
| `BringToFront`           | Move the detail in front of all others              | `element.BringToFront(view)`             |
| `SendBackward`           | Move the detail one step toward the back            | `element.SendBackward(view)`             |
| `SendToBack`             | Move the detail behind all others                   | `element.SendToBack(view)`               |
| `GetDrawOrderForDetails` | Details sorted by current draw order                | `view.GetDrawOrderForDetails(detailIds)` |

## Parts (PartUtils)

| Extension                            | Purpose                                    | Example                                                                                       |
|--------------------------------------|--------------------------------------------|-----------------------------------------------------------------------------------------------|
| `HasAssociatedParts`                 | Whether the element has associated parts   | `element.HasAssociatedParts`                                                                  |
| `GetAssociatedParts`                 | Parts associated with the element          | `element.GetAssociatedParts(includePartsWithAssociatedParts: true, includeAllChildren: true)` |
| `GetAssociatedPartMaker`             | PartMaker associated with the element      | `element.GetAssociatedPartMaker()`                                                            |
| `CreateParts`                        | Create parts from the elements             | `elementIds.CreateParts(document)`                                                            |
| `DivideParts`                        | Create divided parts from parts            | `elementIds.DivideParts(document, intersectingReferenceIds, curveArray, sketchPlaneId)`       |
| `CreateMergedPart`                   | Merge parts into a single part             | `elementIds.CreateMergedPart(document)`                                                       |
| `IsMergedPart`                       | Whether the part is a merge result         | `part.IsMergedPart`                                                                           |
| `GetSplittingCurves`                 | Curves used to create the part             | `part.GetSplittingCurves()`                                                                   |
| `GetSplittingElements`               | Elements used to create the part           | `part.GetSplittingElements()`                                                                 |
| `AreElementsValidForCreateParts`     | Whether elements can make parts            | `elementIds.AreElementsValidForCreateParts(document)`                                         |
| `IsValidForCreateParts`              | Whether a link element can make parts      | `linkElementId.IsValidForCreateParts(document)`                                               |
| `ArePartsValidForDivide`             | Whether parts can be divided               | `elementIds.ArePartsValidForDivide(document)`                                                 |
| `ArePartsValidForMerge`              | Whether parts can be merged                | `elementIds.ArePartsValidForMerge(document)`                                                  |
| `FindMergeableClusters`              | Subsets of elements valid for merge        | `elementIds.FindMergeableClusters(document)`                                                  |
| `GetChainLengthToOriginal`           | Longest divide/merge chain to the original | `part.GetChainLengthToOriginal()`                                                             |
| `GetMergedParts`                     | Source element ids of a merged part        | `part.GetMergedParts()`                                                                       |
| `GetPartMakerMethodToDivideVolumeFw` | Divided-volume accessor of the PartMaker   | `partMaker.GetPartMakerMethodToDivideVolumeFw()`                                              |

## Assembly (AssemblyViewUtils)

| Extension                      | Purpose                                         | Example                                                                               |
|--------------------------------|-------------------------------------------------|---------------------------------------------------------------------------------------|
| `AcquireViews`                 | Transfer assembly views to a sibling assembly   | `sourceAssembly.AcquireViews(targetAssembly)`                                         |
| `Create3DOrthographic`         | Create an orthographic 3D assembly view         | `assemblyInstance.Create3DOrthographic()`                                             |
| `CreateDetailSection`          | Create a detail section assembly view           | `assemblyInstance.CreateDetailSection(AssemblyDetailViewOrientation.ElevationBottom)` |
| `CreateMaterialTakeoff`        | Create a material takeoff assembly view         | `assemblyInstance.CreateMaterialTakeoff()`                                            |
| `CreatePartList`               | Create a part list assembly view                | `assemblyInstance.CreatePartList()`                                                   |
| `CreateSheet`                  | Create a sheet assembly view                    | `assemblyInstance.CreateSheet(titleBlockId)`                                          |
| `CreateSingleCategorySchedule` | Create a single-category schedule assembly view | `assemblyInstance.CreateSingleCategorySchedule(scheduleCategoryId)`                   |

## Mass (MassInstanceUtils)

| Extension                 | Purpose                                       | Example                                     |
|---------------------------|-----------------------------------------------|---------------------------------------------|
| `IsMassFamilyInstance`    | Whether the element is a mass family instance | `massInstance.IsMassFamilyInstance`         |
| `GetMassGrossFloorArea`   | Total occupiable floor area of the mass       | `massInstance.GetMassGrossFloorArea()`      |
| `GetMassGrossSurfaceArea` | Total exterior surface area of the mass       | `massInstance.GetMassGrossSurfaceArea()`    |
| `GetMassGrossVolume`      | Total building volume of the mass             | `massInstance.GetMassGrossVolume()`         |
| `GetMassLevelDataIds`     | MassLevelData ids of the mass                 | `massInstance.GetMassLevelDataIds()`        |
| `GetMassJoinedElementIds` | Elements joined to the mass                   | `massInstance.GetMassJoinedElementIds()`    |
| `GetMassLevelIds`         | Levels associated with the mass               | `massInstance.GetMassLevelIds()`            |
| `AddMassLevelData`        | Associate a level with the mass               | `massInstance.AddMassLevelData(levelId)`    |
| `RemoveMassLevelData`     | Remove a level association                    | `massInstance.RemoveMassLevelData(levelId)` |
