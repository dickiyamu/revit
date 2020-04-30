#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Honeybee.Core.Extensions;
using Honeybee.Revit.CreateModel;
using Honeybee.Revit.CreateModel.Wrappers;
using Newtonsoft.Json;
using NLog;
using DF = DragonflySchema;

// ReSharper disable NonReadonlyMemberInGetHashCode

#endregion

namespace Honeybee.Revit.Schemas
{
    public class Room2D : IBaseObject, ISchema<DF.Room2D>
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [JsonProperty("type")]
        public string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("identifier")]
        public string Identifier { get; set; } = $"Room_{Guid.NewGuid()}";

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("properties")]
        public Room2DPropertiesAbridged 
            Properties { get; set; } = new Room2DPropertiesAbridged();

        [JsonProperty("floor_boundary")]
        public List<Point2D> FloorBoundary { get; set; }

        [JsonProperty("floor_holes")]
        public List<List<Point2D>> FloorHoles { get; set; }

        [JsonProperty("floor_height")]
        public double FloorHeight { get; set; }

        [JsonProperty("floor_to_ceiling_height")]
        public double FloorToCeilingHeight { get; set; }

        [JsonProperty("user_data")]
        public object UserData { get; set; } = new Dictionary<string, object>();

        [JsonProperty("is_ground_contact")]
        public bool IsGroundContact { get; set; }

        [JsonProperty("is_top_exposed")]
        public bool IsTopExposed { get; set; }

        [JsonProperty("boundary_conditions")]
        public List<BoundaryConditionBase> BoundaryConditions { get; set; } = new List<BoundaryConditionBase>();

        [JsonProperty("window_parameters")]
        public List<WindowParameterBase> WindowParameters { get; set; } = new List<WindowParameterBase>();

        //public List<DF.AnyOf<DF.ExtrudedBorder, DF.Overhang, DF.LouversByDistance, DF.LouversByCount>> ShadingParameters { get; set; }

        [JsonIgnore]
        internal Level Level { get; set; }

        [JsonIgnore]
        internal List<AnnotationWrapper> Annotations { get; set; } = new List<AnnotationWrapper>();

        [JsonConstructor]
        public Room2D()
        {
        }

        public Room2D(SpatialElement e)
        {
            DisplayName = e.Name;

            if (e.Document.GetElement(e.LevelId) is Level level)
            {
                FloorHeight = level.Elevation;
                Level = level;
            }

            var doc = e.Document;
            var bOptions = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center
            };
            var tolerance = doc.Application.ShortCurveTolerance;
            var calculator = new SpatialElementGeometryCalculator(doc, bOptions);
            var roomGeo = calculator.CalculateSpatialElementGeometry(e);
            var geo = roomGeo.GetGeometry();
            var bb = geo.GetBoundingBox();
            var height = bb.Max.Z - bb.Min.Z;
            var segments = e.GetBoundarySegments(bOptions);
            var faces = roomGeo.GetGeometry().Faces;
            var offset = e.get_Parameter(BuiltInParameter.ROOM_LOWER_OFFSET).AsDouble();

            var boundary = new List<Point2D>();
            var holes = new List<List<Point2D>>();
            var windows = new List<WindowParameterBase>();
            for (var i = 0; i < segments.Count; i++)
            {
                if (i == 0) // outer boundary
                {
                    foreach (var bs in segments[i])
                    {
                        // (Konrad) Boundary curves have elevation of the level that room base is set to.
                        // They don't account for base offset.
                        var boundaryCurve = bs.GetCurve().Offset(offset);
                        
                        if (boundaryCurve.Length < 0.01)
                            continue; // Exclude tiny curves, they don't produce faces.

                        var face = FindFace(faces, roomGeo, boundaryCurve);
                        if (face == null)
                            continue; // Couldn't find a matching face. Not good.

                        GetGlazingInfo(face, doc, roomGeo, tolerance, out var unused, out var glazingAreas);

                        var faceArea = boundaryCurve.Length * height;
                        var glazingArea = glazingAreas.Sum();
                        var glazingRatio = glazingArea / faceArea;

                        // (Konrad) Number of Boundary points in the list has to match number of Window Parameters.
                        var boundaryPts = GetPoints(boundaryCurve);

                        boundary.AddRange(boundaryPts);
                        windows.AddRange(boundaryPts.Select(x =>
                            Math.Abs(glazingRatio) < 0.01
                                ? (WindowParameterBase) null
                                : new SimpleWindowRatio {WindowRatio = glazingRatio}));
                    }

                    continue;
                }

                var hole = new List<Point2D>();
                foreach (var bs in segments[i])
                {
                    //TODO: Floor Holes need Glazing info processed.

                    var boundaryCurve = bs.GetCurve();
                    var segmentPts = GetPoints(boundaryCurve);

                    hole.AddRange(segmentPts);
                    windows.AddRange(segmentPts.Select(segmentPt => (WindowParameterBase) null));
                }

                holes.Add(hole);
            }

            FloorToCeilingHeight = height;
            FloorBoundary = boundary;
            FloorHoles = holes;
            BoundaryConditions = Enumerable.Range(0, FloorBoundary.Count + FloorHoles.SelectMany(x => x).Count())
                .Select(x => new Outdoors()).Cast<BoundaryConditionBase>().ToList();
            WindowParameters = windows;
        }

        private static void GetGlazingInfo(Face face, Document doc, SpatialElementGeometryResults result, double tolerance, out List<List<XYZ>> glazingPoints, out List<double> glazingAreas)
        {
            glazingPoints = new List<List<XYZ>>();
            glazingAreas = new List<double>();

            if (!(face is PlanarFace))
                return;

            var boundaryFaces = result.GetBoundaryFaceInfo(face);
            foreach (var bFace in boundaryFaces)
            {
                var bElement = doc.GetElement(bFace.SpatialBoundaryElement.HostElementId);
                if (bElement is Wall wall)
                {
                    if (wall.WallType.Kind == WallKind.Curtain)
                    {
                        GetGlazingFromCurtainWall(wall, face, tolerance, ref glazingPoints, ref glazingAreas);
                    }
                    else
                    {
                        GetGlazingFromWindows(wall, face, tolerance, ref glazingPoints, ref glazingAreas);
                    }
                }
            }
        }

        private static void GetGlazingFromWindows(Wall wall, Face face, double tolerance, ref List<List<XYZ>> glazingPts, ref List<double> glazingAreas)
        {
            var doc = wall.Document;
            var inserts = wall.FindInserts(true, false, true, true).Select(doc.GetElement);
            foreach (var insert in inserts)
            {
                if (insert.Category.Id.IntegerValue == BuiltInCategory.OST_Windows.GetHashCode())
                {
                    var winPts = GetGeometryPoints(insert);
                    if (!GetPointsOnFace(face, winPts, out var ptsOnFace, out var uvsOnFace)) continue;
                    if (!GetHull(ptsOnFace, uvsOnFace, tolerance, out var hPts, out var hUvs)) continue;

                    var winArea = GetWindowArea(insert);
                    var hullArea = PolygonArea(hUvs);
                    if (hullArea < winArea * 0.5) continue;

                    var outerEdges = face.GetEdgesAsCurveLoops().First();
                    foreach (var edge in outerEdges)
                    {
                        for (var i = 0; i < hPts.Count; i++)
                        {
                            var pt = hPts[i];
                            if (edge.Distance(pt) >= 0.01) continue;

                            var direction = (edge.GetEndPoint(1) - edge.GetEndPoint(0)).Normalize();
                            var perpendicular = face.ComputeNormal(new UV(0.5, 0.5)).CrossProduct(direction);
                            var offset = 0.1 * perpendicular;
                            var offsetPt = pt + offset;

                            hPts[i] = offsetPt;
                        }
                    }

                    glazingAreas.Add(PolygonArea(hUvs));
                    glazingPts.Add(hPts);
                }
            }
        }

        private static double GetWindowArea(Element insert)
        {
            var winType = (FamilySymbol)insert.Document.GetElement(insert.GetTypeId());

            var furnitureWidthInstance = insert.get_Parameter(BuiltInParameter.FURNITURE_WIDTH)?.AsDouble() ?? 0;
            var furnitureWidthType = winType.get_Parameter(BuiltInParameter.FURNITURE_WIDTH)?.AsDouble() ?? 0;
            var furnWidth = furnitureWidthInstance > 0 ? furnitureWidthInstance : furnitureWidthType;
            var familyWidthInstance = insert.get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM)?.AsDouble() ?? 0;
            var familyWidthType = winType.get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM)?.AsDouble() ?? 0;
            var famWidth = familyWidthInstance > 0 ? familyWidthInstance : familyWidthType;
            var roughWidthInstance = insert.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM)?.AsDouble() ?? 0;
            var roughWidthType = winType.get_Parameter(BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM)?.AsDouble() ?? 0;
            var rWidth = roughWidthInstance > 0 ? roughWidthInstance : roughWidthType;
            var width = rWidth > 0 ? rWidth : famWidth > 0 ? famWidth : furnWidth;

            var furnitureHeightInstance = insert.get_Parameter(BuiltInParameter.FURNITURE_HEIGHT)?.AsDouble() ?? 0;
            var furnitureHeightType = winType.get_Parameter(BuiltInParameter.FURNITURE_HEIGHT)?.AsDouble() ?? 0;
            var furnHeight = furnitureHeightInstance > 0 ? furnitureHeightInstance : furnitureHeightType;
            var familyHeightInstance = insert.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM)?.AsDouble() ?? 0;
            var familyHeightType = winType.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM)?.AsDouble() ?? 0;
            var famHeight = familyHeightInstance > 0 ? familyHeightInstance : familyHeightType;
            var roughHeightInstance = insert.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM)?.AsDouble() ?? 0;
            var roughHeightType = winType.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM)?.AsDouble() ?? 0;
            var rHeight = roughWidthInstance > 0 ? roughHeightInstance : roughHeightType;
            var height = rHeight > 0 ? rHeight : famHeight > 0 ? famHeight : furnHeight;

            var winArea = width * height;

            return winArea;
        }

        private static Face FindFace(IEnumerable faces, SpatialElementGeometryResults result, Curve bCurve)
        {
            foreach (Face f in faces)
            {
                var boundaryFaces = result.GetBoundaryFaceInfo(f).FirstOrDefault();
                if (boundaryFaces != null && (boundaryFaces.SubfaceType == SubfaceType.Top ||
                                              boundaryFaces.SubfaceType == SubfaceType.Bottom))
                {
                    continue; // face is either Top/Bottom so we can skip
                }

                var normal = f.ComputeNormal(new UV(0.5, 0.5));
                if (normal.IsAlmostEqualTo(XYZ.BasisZ) || normal.IsAlmostEqualTo(XYZ.BasisZ.Negate()))
                    continue; // face is either Top/Bottom so we can skip

                var edges = f.GetEdgesAsCurveLoops().First(); // first loop is outer boundary
                if (!edges.Any(x => x.OverlapsWithIn2D(bCurve))) // room's face might be off the floor/level above or offset. if XY matches, we are good.
                    continue; // none of the edges of that face match our curve so we can skip

                return f;
            }

            return null;
        }

        private static void GetGlazingFromCurtainWall(Wall wall, Face face, double tolerance, ref List<List<XYZ>> glazingPts, ref List<double> glazingAreas)
        {
            var doc = wall.Document;
            var cGrid = wall.CurtainGrid;
            var panels = cGrid.GetPanelIds().Select(x => doc.GetElement(x));

            foreach (var panel in panels)
            {
                var points = GetGeometryPoints(panel);
                if (!GetPointsOnFace(face, points, out var ptsOnFace, out var uvsOnFace)) continue;
                if (!GetHull(ptsOnFace, uvsOnFace, tolerance, out var hPts, out var hUvs)) continue;

                var outerEdges = face.GetEdgesAsCurveLoops().First();
                foreach (var edge in outerEdges)
                {
                    for (var i = 0; i < hPts.Count; i++)
                    {
                        var pt = hPts[i];
                        if (edge.Distance(pt) >= 0.01) continue;

                        var direction = (edge.GetEndPoint(1) - edge.GetEndPoint(0)).Normalize();
                        var perpendicular = face.ComputeNormal(new UV(0.5, 0.5)).CrossProduct(direction);
                        var offset = 0.1 * perpendicular;
                        var offsetPt = pt + offset;

                        hPts[i] = offsetPt;
                    }
                }

                glazingAreas.Add(PolygonArea(hUvs));
                glazingPts.Add(hPts);
            }
        }

        private static double PolygonArea(IList<UV> polygon)
        {
            int i, j;
            double area = 0;

            for (i = 0; i < polygon.Count; i++)
            {
                j = (i + 1) % polygon.Count;

                area += polygon[i].U * polygon[j].V;
                area -= polygon[i].V * polygon[j].U;
            }

            area /= 2;
            return (area < 0 ? -area : area);
        }

        private static bool GetHull(List<XYZ> pts, List<UV> uvs, double tolerance, out List<XYZ> hullPts, out List<UV> hullUvs)
        {
            hullPts = new List<XYZ>();
            hullUvs = new List<UV>();

            if (!pts.Any() || !uvs.Any())
                return false;

            try
            {
                var hullPoints = uvs.Select(x => new HullPoint(x.U, x.V)).ToList();
                var hull = ConvexHull.MakeHull(hullPoints);

                var hUvs = hull.Select(x => new UV(x.x, x.y)).ToList();
                var hPts = hUvs.Select(x => pts[uvs.FindIndex(y => y.IsAlmostEqualTo(x))]).ToList();

                var indexToRemove = -1;

                Restart:

                if (indexToRemove != -1)
                {
                    hPts.RemoveAt(indexToRemove);
                    hUvs.RemoveAt(indexToRemove);

                    // ReSharper disable once RedundantAssignment
                    indexToRemove = -1;
                }

                for (var i = 0; i < hPts.Count; i++)
                {
                    var start = hPts[i];
                    XYZ middle;
                    XYZ end;
                    int middleIndex;
                    if (i + 2 == hPts.Count)
                    {
                        middle = hPts[i + 1];
                        middleIndex = i + 1;
                        end = hPts[0];
                    }
                    else if (i + 1 == hPts.Count)
                    {
                        middle = hPts[0];
                        middleIndex = 0;
                        end = hPts[1];
                    }
                    else
                    {
                        middle = hPts[i + 1];
                        middleIndex = i + 1;
                        end = hPts[i + 2];
                    }

                    if (start.DistanceTo(end) < tolerance) continue;

                    var line = Line.CreateBound(start, end);
                    var intResult = line.Project(middle);
                    if (intResult.Distance > 0.01) continue;

                    indexToRemove = middleIndex;
                    goto Restart;
                }

                hullPts = hPts;
                hullUvs = hUvs;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool GetPointsOnFace(Face face, List<XYZ> pts, out List<XYZ> ptsOnFace, out List<UV> uvsOnFace)
        {
            var onFace = new HashSet<XYZ>();
            var onFaceUvs = new HashSet<UV>();
            foreach (var pt in pts)
            {
                var intResult = face.Project(pt);
                if (intResult == null) continue;

                if (onFace.Add(intResult.XYZPoint))
                    onFaceUvs.Add(intResult.UVPoint.Negate());
            }

            ptsOnFace = onFace.ToList();
            uvsOnFace = onFaceUvs.ToList();

            return ptsOnFace.Any() && uvsOnFace.Any();
        }

        private static List<XYZ> GetGeometryPoints(Element e)
        {
            var pts = new List<XYZ>();
            using (var opt = new Options())
            {
                opt.IncludeNonVisibleObjects = true;
                using (var geom = e.get_Geometry(opt))
                {
                    ExtractPtsRecursively(geom, ref pts);
                }
            }

            return pts;
        }

        public DF.Room2D ToDragonfly()
        {
            return new DF.Room2D(
                Identifier,
                FloorBoundary.ToDragonfly(),
                FloorHeight,
                FloorToCeilingHeight,
                Properties.ToDragonfly(),
                DisplayName,
                null,
                Type,
                FloorHoles.ToDragonfly(),
                IsGroundContact,
                IsTopExposed,
                BoundaryConditions.ToDragonfly(),
                WindowParameters.ToDragonfly()
            );
        }

        public override bool Equals(object obj)
        {
            return obj is Room2D item && Identifier.Equals(item.Identifier);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        #region Utilities

        private static void ExtractPtsRecursively(GeometryElement geo, ref List<XYZ> pts, bool includeLines = false)
        {
            foreach (var g in geo)
            {
                var instGeo = g as GeometryInstance;
                if (instGeo != null)
                {
                    ExtractPtsRecursively(instGeo.GetInstanceGeometry(), ref pts, includeLines);
                    continue;
                }

                var solidGeo = g as Solid;
                if (solidGeo != null)
                {
                    foreach (Face f in solidGeo.Faces)
                    {
                        ProcessFace(f, ref pts);
                    }

                    continue;
                }

                var faceGeo = g as Face;
                if (faceGeo != null) ProcessFace(faceGeo, ref pts);

                var meshGeo = g as Mesh;
                if (meshGeo != null) pts.AddRange(meshGeo.Vertices);

                if (!includeLines) continue;

                var lineGeo = g as Curve;
                if (lineGeo != null && lineGeo.IsBound)
                    pts.AddRange(new List<XYZ> {lineGeo.GetEndPoint(0), lineGeo.GetEndPoint(1)});
            }
        }

        private static readonly double[] _params = {0d, 0.2, 0.4, 0.6, 0.8};

        private static void ProcessFace(Face f, ref List<XYZ> pts)
        {
            foreach (EdgeArray edges in f.EdgeLoops)
            {
                foreach (Edge e in edges)
                {
                    pts.AddRange(_params.Select(p => e.Evaluate(p)));
                }
            }
        }

        private static List<List<Point2D>> GetHoles(IList<IList<BoundarySegment>> bs)
        {
            var holes = new List<List<Point2D>>();
            for (var i = 1; i < bs.Count; i++)
            {
                // (Konrad) All loops starting from index 1 are floor holes.
                var holeBoundary = bs[i];
                holes.Add(GetPoints(holeBoundary));
            }

            return holes;
        }

        private static List<Point2D> GetBoundary(IEnumerable<IList<BoundarySegment>> bs)
        {
            var boundary = new List<Point2D>();
            var outerBoundary = bs.FirstOrDefault();
            if (outerBoundary == null) return boundary;

            boundary.AddRange(GetPoints(outerBoundary));

            return boundary;
        }

        private static double GetCeilingHeight(SpatialElement se)
        {
            try
            {
                var view = new FilteredElementCollector(se.Document)
                    .OfClass(typeof(View3D))
                    .Cast<View3D>()
                    .FirstOrDefault(x => !x.IsTemplate);
                if (view == null)
                    return se.GetUnboundHeight();

                var basePt = se.GetLocationPoint();
                if (basePt == null)
                    return se.GetUnboundHeight();

                var direction = new XYZ(0, 0, 1);
                var filter = new ElementClassFilter(typeof(Ceiling));
                var refIntersector = new ReferenceIntersector(filter, FindReferenceTarget.Face, view);
                var refWithContext = refIntersector.FindNearest(basePt, direction);
                if (refWithContext == null)
                {
                    // (Konrad) There is no Ceiling. What about a Floor (exposed ceiling)?
                    basePt += new XYZ(0, 0, 0.1); // floor/bottom of room intersect let's move point up
                    filter = new ElementClassFilter(typeof(Floor));
                    refIntersector = new ReferenceIntersector(filter, FindReferenceTarget.Face, view);
                    refWithContext = refIntersector.FindNearest(basePt, direction);
                }

                if (refWithContext == null)
                {
                    // (Konrad) There is no Floor. What about Roof (exposed ceiling on top floor)?
                    filter = new ElementClassFilter(typeof(RoofBase));
                    refIntersector = new ReferenceIntersector(filter, FindReferenceTarget.Face, view);
                    refWithContext = refIntersector.FindNearest(basePt, direction);
                }

                if (refWithContext == null)
                    return se.GetUnboundHeight();

                var reference = refWithContext.GetReference();
                var intersection = reference.GlobalPoint;
                var line = Line.CreateBound(basePt, intersection);
                var height = line.Length;

                return height;
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                return 0d;
            }
        }

        private static List<Point2D> GetPoints(Curve curve)
        {
            var curves = new List<Point2D>();
            switch (curve)
            {
                case Line line:
                    curves.Add(new Point2D(line.GetEndPoint(0)));
                    break;
                case Arc arc:
                    curves.Add(new Point2D(arc.Evaluate(0, true)));
                    curves.Add(new Point2D(arc.Evaluate(0.25, true)));
                    curves.Add(new Point2D(arc.Evaluate(0.5, true)));
                    curves.Add(new Point2D(arc.Evaluate(0.75, true)));
                    break;
                case CylindricalHelix unused:
                case Ellipse unused1:
                case HermiteSpline unused2:
                case NurbSpline unused3:
                    break;
            }

            return curves;
        }

        private static List<Point2D> GetPoints(IEnumerable<BoundarySegment> segments)
        {
            var curves = new List<Point2D>();
            foreach (var bs in segments)
            {
                var curve = bs.GetCurve();
                switch (curve)
                {
                    case Line line:
                        curves.Add(new Point2D(line.GetEndPoint(0)));
                        break;
                    case Arc arc:
                        curves.Add(new Point2D(arc.Evaluate(0, true)));
                        curves.Add(new Point2D(arc.Evaluate(0.25, true)));
                        curves.Add(new Point2D(arc.Evaluate(0.5, true)));
                        curves.Add(new Point2D(arc.Evaluate(0.75, true)));
                        break;
                    case CylindricalHelix unused:
                    case Ellipse unused1:
                    case HermiteSpline unused2:
                    case NurbSpline unused3:
                        break;
                }
            }

            return curves;
        }

        private static List<WindowParameterBase> GetWindowParameters(IList<IList<BoundarySegment>> bs, Document doc)
        {
            var wParams = new List<WindowParameterBase>();
            var outerBoundary = bs.FirstOrDefault();
            if (outerBoundary == null) return wParams;

            for (var i = 0; i < bs.Count; i++)
            {
                var loop = bs[i];
                for (var k = 0; k < loop.Count; k++)
                {
                    var segment = bs[i][k];
                    var bElement = doc.GetElement(segment.ElementId);
                    if (bElement == null) continue;

                    if (bElement is Wall wall)
                    {
                        var inserts = wall.FindInserts(true, false, true, true)
                            .Select(doc.GetElement)
                            .Where(x => x.Category.Id.IntegerValue == BuiltInCategory.OST_Windows.GetHashCode())
                            .ToList();
                    }
                }
            }

            return wParams;
        }

        #endregion
    }
}