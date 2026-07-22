# Geometry

Extensions on solids, bounding boxes, curves, points, and view geometry.
Each `## Heading (RawClass)` names the raw Revit static this domain replaces; call the extension on the receiver shown instead.
Headings with no raw class add behavior the raw API has no single call for.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

## Line distance

- `first.Distance(second)`;

## Bounding box

- `boundingBox.ComputeCentroid()`;
- `boundingBox.ComputeVertices()`;
- `boundingBox.ComputeVolume()`;
- `boundingBox.ComputeSurfaceArea()`;
- `boundingBox.Contains(new XYZ(1, 1, 1), strict: true)`;
- `first.Overlaps(second)`;

## Curve and point

- `line.SetCoordinateX(1)`;
- `line.SetCoordinateY(1)`;
- `line.SetCoordinateZ(1)`;
- `point.SetX(1)`;
- `point.SetY(1)`;
- `point.SetZ(1)`;

## CurveElement (face regions, projection)

- `curveElement.GetHostFace()`;
- `curveElement.GetProjectionType()`;
- `curveElement.SetProjectionType(CurveProjectionType.FromTopDown)`;
- `curveElement.GetSketchOnSurface()`;
- `curveElement.SetSketchOnSurface(sketchOnSurface: true)`;
- `CurveElement.CreateArcThroughPoints(document, startPoint, endPoint, interiorPoint)`;
- `CurveElement.AddCurvesToFaceRegion(document, curveElementIds)`;
- `CurveElement.CreateRectangle(document, startPoint, endPoint, projectionType, boundaryReferenceLines, boundaryCurvesFollowSurface, out createdCurvesIds, out createdCornersIds)`;
- `CurveElement.ValidateForFaceRegions(document, curveElemIds)`;
- `CurveLoop.IsValidHorizontalBoundary(curveLoops)`;
- `CurveLoop.IsValidBoundaryOnSketchPlane(sketchPlane, curveLoops)`;
- `CurveLoop.IsValidBoundaryOnView(document, viewId, curveLoops)`;
- `reference.GetFaceRegions(document)`;

## Solid create and transform (GeometryCreationUtilities, SolidUtils)

- `geometry.IsSolid`;
- `geometry.IsNonSolid`;
- `solid.Clone()`;
- `solid.CreateTransformed(Transform.CreateRotationAtPoint())`;
- `Solid.CreateBlendGeometry(firstLoop, secondLoop)`;
- `Solid.CreateExtrusionGeometry(profileLoops, extrusionDirection, extrusionDistance)`;
- `Solid.CreateRevolvedGeometry(coordinateFrame, profileLoops, startAngle, endAngle)`;
- `Solid.CreateSweptGeometry(sweepPath, pathAttachmentCurveIndex, pathAttachmentParameter, profileLoops)`;
- `Solid.CreateSweptBlendGeometry(pathCurve, pathParameters, profileLoops, vertexPairs)`;
- `Solid.CreateFixedReferenceSweptGeometry(sweepPath, pathAttachmentCurveIndex, pathAttachmentParameter, profileLoops, fixedReferenceDirection)`;
- `Solid.CreateLoftGeometry(profileLoops, solidOptions)`;

## Solid boolean, split, cut (BooleanOperationsUtils, SolidUtils)

- `solid.ExecuteBooleanOperation(other, BooleanOperationsType.Union)`;
- `solid.ExecuteBooleanOperationModifyingOriginalSolid(other, BooleanOperationsType.Union)`;
- `solid.SplitVolumes()`;
- `solid.CutWithHalfSpace(plane)`;
- `solid.CutWithHalfSpaceModifyingOriginalSolid(plane)`;
- `solid.ComputeIsGeometricallyClosed(profileLoops, solidOptions)`;
- `solid.ComputeIsTopologicallyClosed(profileLoops, solidOptions)`;

## Tessellation (FacetingUtils)

- `solid.IsValidForTessellation`;
- `solid.TessellateSolidOrShell(tessellationControls)`;
- `triangulation.ConvertTrianglesToQuads()`;
- `edgeEndPoint.FindAllEdgeEndPointsAtVertex()`;

## View geometry

- `view.GetTransformFromViewToView(otherView)`;
- `element.GetReferencedViewId()`;
- `element.ChangeReferencedView(desiredViewId)`;
- `category.GetSsePointVisibility(document)`;
- `category.SetSsePointVisibility(document, isVisible: true)`;
- `view.CreateSpatialFieldManager(numberOfMeasurements: 69)`;
- `view.GetSpatialFieldManager()`;
