# Geometry

Extensions on solids, bounding boxes, curves, points, and view geometry.
Each `## Heading (RawClass)` names the raw Revit static this domain replaces; call the extension on the receiver shown instead.
Headings with no raw class add behavior the raw API has no single call for.
A member missing from your build means the installed `Nice3point.Revit.Extensions` version predates it.

## Line distance

| Extension  | Purpose                            | Example                  |
|------------|------------------------------------|--------------------------|
| `Distance` | Distance between two endless lines | `first.Distance(second)` |

## Bounding box

| Extension            | Purpose                                 | Example                                                |
|----------------------|-----------------------------------------|--------------------------------------------------------|
| `ComputeCentroid`    | Geometric center of the box             | `boundingBox.ComputeCentroid()`                        |
| `ComputeVertices`    | The eight corner coordinates            | `boundingBox.ComputeVertices()`                        |
| `ComputeVolume`      | Volume enclosed by the box              | `boundingBox.ComputeVolume()`                          |
| `ComputeSurfaceArea` | Total surface area of the box           | `boundingBox.ComputeSurfaceArea()`                     |
| `Contains`           | Whether the box contains a point or box | `boundingBox.Contains(new XYZ(1, 1, 1), strict: true)` |
| `Overlaps`           | Whether two boxes overlap               | `first.Overlaps(second)`                               |

## Curve and point

| Extension        | Purpose                        | Example                  |
|------------------|--------------------------------|--------------------------|
| `SetCoordinateX` | Copy of the curve with a new X | `line.SetCoordinateX(1)` |
| `SetCoordinateY` | Copy of the curve with a new Y | `line.SetCoordinateY(1)` |
| `SetCoordinateZ` | Copy of the curve with a new Z | `line.SetCoordinateZ(1)` |
| `SetX`           | Copy of the point with a new X | `point.SetX(1)`          |
| `SetY`           | Copy of the point with a new Y | `point.SetY(1)`          |
| `SetZ`           | Copy of the point with a new Z | `point.SetZ(1)`          |

## CurveElement (face regions, projection)

| Extension                      | Purpose                                        | Example                                                                                                                                                                          |
|--------------------------------|------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `GetHostFace`                  | Host face the curve element is added to        | `curveElement.GetHostFace()`                                                                                                                                                     |
| `GetProjectionType`            | Projection type of the curve element           | `curveElement.GetProjectionType()`                                                                                                                                               |
| `SetProjectionType`            | Set the projection type                        | `curveElement.SetProjectionType(CurveProjectionType.FromTopDown)`                                                                                                                |
| `GetSketchOnSurface`           | Curve-to-face relationship                     | `curveElement.GetSketchOnSurface()`                                                                                                                                              |
| `SetSketchOnSurface`           | Set the curve-to-face relationship             | `curveElement.SetSketchOnSurface(sketchOnSurface: true)`                                                                                                                         |
| `CreateArcThroughPoints`       | Arc through reference points                   | `CurveElement.CreateArcThroughPoints(document, startPoint, endPoint, interiorPoint)`                                                                                             |
| `AddCurvesToFaceRegion`        | Add curve elements to face regions             | `CurveElement.AddCurvesToFaceRegion(document, curveElementIds)`                                                                                                                  |
| `CreateRectangle`              | Rectangle on a face or sketch plane            | `CurveElement.CreateRectangle(document, startPoint, endPoint, projectionType, boundaryReferenceLines, boundaryCurvesFollowSurface, out createdCurvesIds, out createdCornersIds)` |
| `ValidateForFaceRegions`       | Whether curves can define face regions         | `CurveElement.ValidateForFaceRegions(document, curveElemIds)`                                                                                                                    |
| `IsValidHorizontalBoundary`    | Whether loops form a valid horizontal boundary | `CurveLoop.IsValidHorizontalBoundary(curveLoops)`                                                                                                                                |
| `IsValidBoundaryOnSketchPlane` | Whether loops form a valid boundary on a plane | `CurveLoop.IsValidBoundaryOnSketchPlane(sketchPlane, curveLoops)`                                                                                                                |
| `IsValidBoundaryOnView`        | Whether loops form a valid boundary on a view  | `CurveLoop.IsValidBoundaryOnView(document, viewId, curveLoops)`                                                                                                                  |
| `GetFaceRegions`               | Face regions in the existing face              | `reference.GetFaceRegions(document)`                                                                                                                                             |

## Solid create and transform (GeometryCreationUtilities, SolidUtils)

| Extension                           | Purpose                                    | Example                                                                                                                                        |
|-------------------------------------|--------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------|
| `IsSolid`                           | Whether the geometry object is a solid     | `geometry.IsSolid`                                                                                                                             |
| `IsNonSolid`                        | Whether the geometry object is not a solid | `geometry.IsNonSolid`                                                                                                                          |
| `Clone`                             | Copy of the solid                          | `solid.Clone()`                                                                                                                                |
| `CreateTransformed`                 | Transformed copy of the solid              | `solid.CreateTransformed(Transform.CreateRotationAtPoint())`                                                                                   |
| `CreateBlendGeometry`               | Solid blending two curve loops             | `Solid.CreateBlendGeometry(firstLoop, secondLoop)`                                                                                             |
| `CreateExtrusionGeometry`           | Solid extruded from coplanar loops         | `Solid.CreateExtrusionGeometry(profileLoops, extrusionDirection, extrusionDistance)`                                                           |
| `CreateRevolvedGeometry`            | Solid of revolution about an axis          | `Solid.CreateRevolvedGeometry(coordinateFrame, profileLoops, startAngle, endAngle)`                                                            |
| `CreateSweptGeometry`               | Solid swept along a path                   | `Solid.CreateSweptGeometry(sweepPath, pathAttachmentCurveIndex, pathAttachmentParameter, profileLoops)`                                        |
| `CreateSweptBlendGeometry`          | Solid swept and blended along a curve      | `Solid.CreateSweptBlendGeometry(pathCurve, pathParameters, profileLoops, vertexPairs)`                                                         |
| `CreateFixedReferenceSweptGeometry` | Swept solid with a fixed profile direction | `Solid.CreateFixedReferenceSweptGeometry(sweepPath, pathAttachmentCurveIndex, pathAttachmentParameter, profileLoops, fixedReferenceDirection)` |
| `CreateLoftGeometry`                | Solid or shell lofted through loops        | `Solid.CreateLoftGeometry(profileLoops, solidOptions)`                                                                                         |

## Solid boolean, split, cut (BooleanOperationsUtils, SolidUtils)

| Extension                                       | Purpose                                         | Example                                                                                   |
|-------------------------------------------------|-------------------------------------------------|-------------------------------------------------------------------------------------------|
| `ExecuteBooleanOperation`                       | Union, intersect, or difference of two solids   | `solid.ExecuteBooleanOperation(other, BooleanOperationsType.Union)`                       |
| `ExecuteBooleanOperationModifyingOriginalSolid` | Boolean that mutates the original solid         | `solid.ExecuteBooleanOperationModifyingOriginalSolid(other, BooleanOperationsType.Union)` |
| `SplitVolumes`                                  | Split a solid into separate solids              | `solid.SplitVolumes()`                                                                    |
| `CutWithHalfSpace`                              | Intersect with the positive side of a plane     | `solid.CutWithHalfSpace(plane)`                                                           |
| `CutWithHalfSpaceModifyingOriginalSolid`        | Keep only the positive side, mutating the solid | `solid.CutWithHalfSpaceModifyingOriginalSolid(plane)`                                     |
| `ComputeIsGeometricallyClosed`                  | Whether the solid is geometrically closed       | `solid.ComputeIsGeometricallyClosed(profileLoops, solidOptions)`                          |
| `ComputeIsTopologicallyClosed`                  | Whether the solid is topologically closed       | `solid.ComputeIsTopologicallyClosed(profileLoops, solidOptions)`                          |

## Tessellation (FacetingUtils)

| Extension                      | Purpose                                       | Example                                              |
|--------------------------------|-----------------------------------------------|------------------------------------------------------|
| `IsValidForTessellation`       | Whether the solid or shell can be tessellated | `solid.IsValidForTessellation`                       |
| `TessellateSolidOrShell`       | Triangulate a solid or open shell             | `solid.TessellateSolidOrShell(tessellationControls)` |
| `ConvertTrianglesToQuads`      | Replace coplanar triangle pairs with quads    | `triangulation.ConvertTrianglesToQuads()`            |
| `FindAllEdgeEndPointsAtVertex` | Edge end points at a shared vertex            | `edgeEndPoint.FindAllEdgeEndPointsAtVertex()`        |

## View geometry

| Extension                    | Purpose                                      | Example                                                     |
|------------------------------|----------------------------------------------|-------------------------------------------------------------|
| `GetTransformFromViewToView` | Transform applied when copying between views | `view.GetTransformFromViewToView(otherView)`                |
| `GetReferencedViewId`        | Id of the view a reference view points to    | `element.GetReferencedViewId()`                             |
| `ChangeReferencedView`       | Point a reference view at another view       | `element.ChangeReferencedView(desiredViewId)`               |
| `GetSsePointVisibility`      | SSE point visibility for a category          | `category.GetSsePointVisibility(document)`                  |
| `SetSsePointVisibility`      | Set SSE point visibility for a category      | `category.SetSsePointVisibility(document, isVisible: true)` |
| `CreateSpatialFieldManager`  | Create a SpatialFieldManager for the view    | `view.CreateSpatialFieldManager(numberOfMeasurements: 69)`  |
| `GetSpatialFieldManager`     | Existing SpatialFieldManager for the view    | `view.GetSpatialFieldManager()`                             |
