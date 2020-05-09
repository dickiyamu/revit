#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Honeybee.Core.Extensions;
using Honeybee.Revit.CreateModel;
using Honeybee.Revit.CreateModel.Wrappers;
using Honeybee.Revit.Schemas.Honeybee;
using Newtonsoft.Json;
using NLog;
using DF = DragonflySchema;
using HB = HoneybeeSchema;
using RVT = Autodesk.Revit.DB;
// ReSharper disable NonReadonlyMemberInGetHashCode

#endregion

namespace Honeybee.Revit.Schemas
{
    public class Room2D : IBaseObject, ISchema<DF.Room2D, HB.Room>
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
        public Room2DPropertiesAbridged Properties { get; set; } = new Room2DPropertiesAbridged();

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

        [JsonProperty("faces")]
        public List<Face> Faces { get; set; } = new List<Face>();

        [JsonIgnore]
        internal RVT.Level Level { get; set; }

        [JsonIgnore]
        internal List<AnnotationWrapper> Annotations { get; set; } = new List<AnnotationWrapper>();

        [JsonConstructor]
        public Room2D()
        {
        }

        public Room2D(RVT.SpatialElement e)
        {
            DisplayName = e.Name;

            if (e.Document.GetElement(e.LevelId) is RVT.Level level)
            {
                FloorHeight = level.Elevation;
                Level = level;
            }

            var doc = e.Document;
            var bOptions = new RVT.SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = RVT.SpatialElementBoundaryLocation.Center
            };
            var tolerance = doc.Application.ShortCurveTolerance;
            var calculator = new RVT.SpatialElementGeometryCalculator(doc, bOptions);
            var roomGeo = calculator.CalculateSpatialElementGeometry(e);
            var geo = roomGeo.GetGeometry();
            var bb = geo.GetBoundingBox();
            var height = bb.Max.Z - bb.Min.Z;
            var segments = e.GetBoundarySegments(bOptions);
            var faces = roomGeo.GetGeometry().Faces;
            var offset = e.get_Parameter(RVT.BuiltInParameter.ROOM_LOWER_OFFSET).AsDouble();

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
                        var boundaryPts = GeometryUtils.GetPoints(boundaryCurve);

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
                    var segmentPts = GeometryUtils.GetPoints(boundaryCurve);

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

            var hbFaces = new List<Face>();
            foreach (RVT.Face face in faces)
            {
                if (face is RVT.CylindricalFace) // round columns only
                {
                    hbFaces.AddRange(PlanarizeCylindricalFace(face));
                }
                else
                {
                    var hbFace = new Face(face);

                    GetGlazingInfo(face, doc, roomGeo, tolerance, out var glazingPts, out var unused);

                    var apertures = glazingPts.Select(x => new Aperture(x.Select(y => new Point3D(y)).ToList()))
                        .ToList();
                    hbFace.Apertures = apertures.Any() ? apertures : null;

                    var boundaryFaces = roomGeo.GetBoundaryFaceInfo(face).FirstOrDefault();
                    if (boundaryFaces != null)
                    {
                        switch (boundaryFaces.SubfaceType)
                        {
                            case RVT.SubfaceType.Bottom:
                                hbFace.FaceType = HB.Face.FaceTypeEnum.Floor;
                                break;
                            case RVT.SubfaceType.Top:
                                hbFace.FaceType = HB.Face.FaceTypeEnum.RoofCeiling;
                                break;
                            case RVT.SubfaceType.Side:
                                hbFace.FaceType = HB.Face.FaceTypeEnum.Wall;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        // (Konrad) We don't really know what type of face it is.
                        // TODO: This is probably an Air Boundary/Room Separation Line.
                        hbFace.FaceType = HB.Face.FaceTypeEnum.Wall;
                    }

                    hbFaces.Add(hbFace);
                }
            }

            Faces = hbFaces;
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
                null, // user data
                FloorHoles.ToDragonfly(),
                IsGroundContact,
                IsTopExposed,
                BoundaryConditions.ToDragonfly(),
                WindowParameters.ToDragonfly(),
                null // shading params
            );
        }

        public HB.Room ToHoneybee()
        {
            return new HB.Room(
                Identifier,
                Faces.Select(x => x.ToHoneybee()).ToList(),
                Properties.ToHoneybee(),
                DisplayName,
                null, // user data
                null, // indoor shades
                null, // outdoor shades
                1 // multiplier
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

        private static List<Face> PlanarizeCylindricalFace(RVT.Face face)
        {
            var cLoop = face.GetEdgesAsCurveLoops().First().ToList();
            if (cLoop.Count < 4 || cLoop.Count(x => x is RVT.Arc) != 2)
                return new List<Face>();

            var bottom = cLoop.First(x => x is RVT.Arc);
            var top = cLoop.Last(x => x is RVT.Arc);

            var b1 = bottom.Evaluate(0, true);
            var b2 = bottom.Evaluate(0.25, true);
            var b3 = bottom.Evaluate(0.5, true);
            var b4 = bottom.Evaluate(0.75, true);
            var b5 = bottom.Evaluate(1, true);

            var t1 = top.Evaluate(0, true);
            var t2 = top.Evaluate(0.25, true);
            var t3 = top.Evaluate(0.5, true);
            var t4 = top.Evaluate(0.75, true);
            var t5 = top.Evaluate(1, true);

            var f1 = new Face(new List<Point3D>
            {
                new Point3D(b1),
                new Point3D(b2),
                new Point3D(t4),
                new Point3D(t5)
            }, new List<List<Point3D>>())
            {
                FaceType = HB.Face.FaceTypeEnum.Wall
            };

            var f2 = new Face(new List<Point3D>
            {
                new Point3D(b2),
                new Point3D(b3),
                new Point3D(t3),
                new Point3D(t4)
            }, new List<List<Point3D>>())
            {
                FaceType = HB.Face.FaceTypeEnum.Wall
            };

            var f3 = new Face(new List<Point3D>
            {
                new Point3D(b3),
                new Point3D(b4),
                new Point3D(t2),
                new Point3D(t3)
            }, new List<List<Point3D>>())
            {
                FaceType = HB.Face.FaceTypeEnum.Wall
            };

            var f4 = new Face(new List<Point3D>
            {
                new Point3D(b4),
                new Point3D(b5),
                new Point3D(t1),
                new Point3D(t2)
            }, new List<List<Point3D>>())
            {
                FaceType = HB.Face.FaceTypeEnum.Wall
            };

            return new List<Face> {f1, f2, f3, f4};
        }

        private static void GetGlazingInfo(RVT.Face face, RVT.Document doc, RVT.SpatialElementGeometryResults result, double tolerance, out List<List<RVT.XYZ>> glazingPoints, out List<double> glazingAreas)
        {
            glazingPoints = new List<List<RVT.XYZ>>();
            glazingAreas = new List<double>();

            if (!(face is RVT.PlanarFace))
                return;

            var boundaryFaces = result.GetBoundaryFaceInfo(face);
            foreach (var bFace in boundaryFaces)
            {
                var bElement = doc.GetElement(bFace.SpatialBoundaryElement.HostElementId);
                if (bElement is RVT.Wall wall)
                {
                    if (wall.WallType.Kind == RVT.WallKind.Curtain)
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

        private static void GetGlazingFromWindows(RVT.Wall wall, RVT.Face face, double tolerance, ref List<List<RVT.XYZ>> glazingPts, ref List<double> glazingAreas)
        {
            var doc = wall.Document;
            var inserts = wall.FindInserts(true, false, true, true).Select(doc.GetElement);
            foreach (var insert in inserts)
            {
                if (insert.Category.Id.IntegerValue == RVT.BuiltInCategory.OST_Windows.GetHashCode())
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
                            var perpendicular = face.ComputeNormal(new RVT.UV(0.5, 0.5)).CrossProduct(direction);
                            var offset = 0.1 * perpendicular;
                            var offsetPt = pt + offset;

                            hPts[i] = offsetPt;
                        }
                    }

                    if (hPts.Count < 3) continue;

                    glazingAreas.Add(PolygonArea(hUvs));
                    glazingPts.Add(hPts);
                }
            }
        }

        private static double GetWindowArea(RVT.Element insert)
        {
            var winType = (RVT.FamilySymbol)insert.Document.GetElement(insert.GetTypeId());

            var furnitureWidthInstance = insert.get_Parameter(RVT.BuiltInParameter.FURNITURE_WIDTH)?.AsDouble() ?? 0;
            var furnitureWidthType = winType.get_Parameter(RVT.BuiltInParameter.FURNITURE_WIDTH)?.AsDouble() ?? 0;
            var furnWidth = furnitureWidthInstance > 0 ? furnitureWidthInstance : furnitureWidthType;
            var familyWidthInstance = insert.get_Parameter(RVT.BuiltInParameter.FAMILY_WIDTH_PARAM)?.AsDouble() ?? 0;
            var familyWidthType = winType.get_Parameter(RVT.BuiltInParameter.FAMILY_WIDTH_PARAM)?.AsDouble() ?? 0;
            var famWidth = familyWidthInstance > 0 ? familyWidthInstance : familyWidthType;
            var roughWidthInstance = insert.get_Parameter(RVT.BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM)?.AsDouble() ?? 0;
            var roughWidthType = winType.get_Parameter(RVT.BuiltInParameter.FAMILY_ROUGH_WIDTH_PARAM)?.AsDouble() ?? 0;
            var rWidth = roughWidthInstance > 0 ? roughWidthInstance : roughWidthType;
            var width = rWidth > 0 ? rWidth : famWidth > 0 ? famWidth : furnWidth;

            var furnitureHeightInstance = insert.get_Parameter(RVT.BuiltInParameter.FURNITURE_HEIGHT)?.AsDouble() ?? 0;
            var furnitureHeightType = winType.get_Parameter(RVT.BuiltInParameter.FURNITURE_HEIGHT)?.AsDouble() ?? 0;
            var furnHeight = furnitureHeightInstance > 0 ? furnitureHeightInstance : furnitureHeightType;
            var familyHeightInstance = insert.get_Parameter(RVT.BuiltInParameter.FAMILY_HEIGHT_PARAM)?.AsDouble() ?? 0;
            var familyHeightType = winType.get_Parameter(RVT.BuiltInParameter.FAMILY_HEIGHT_PARAM)?.AsDouble() ?? 0;
            var famHeight = familyHeightInstance > 0 ? familyHeightInstance : familyHeightType;
            var roughHeightInstance = insert.get_Parameter(RVT.BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM)?.AsDouble() ?? 0;
            var roughHeightType = winType.get_Parameter(RVT.BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM)?.AsDouble() ?? 0;
            var rHeight = roughWidthInstance > 0 ? roughHeightInstance : roughHeightType;
            var height = rHeight > 0 ? rHeight : famHeight > 0 ? famHeight : furnHeight;

            var winArea = width * height;

            return winArea;
        }

        private static RVT.Face FindFace(IEnumerable faces, RVT.SpatialElementGeometryResults result, RVT.Curve bCurve)
        {
            foreach (RVT.Face f in faces)
            {
                var boundaryFaces = result.GetBoundaryFaceInfo(f).FirstOrDefault();
                if (boundaryFaces != null && (boundaryFaces.SubfaceType == RVT.SubfaceType.Top ||
                                              boundaryFaces.SubfaceType == RVT.SubfaceType.Bottom))
                {
                    continue; // face is either Top/Bottom so we can skip
                }

                var normal = f.ComputeNormal(new RVT.UV(0.5, 0.5));
                if (normal.IsAlmostEqualTo(RVT.XYZ.BasisZ) || normal.IsAlmostEqualTo(RVT.XYZ.BasisZ.Negate()))
                    continue; // face is either Top/Bottom so we can skip

                var edges = f.GetEdgesAsCurveLoops().First(); // first loop is outer boundary
                if (!edges.Any(x => x.OverlapsWithIn2D(bCurve))) // room's face might be off the floor/level above or offset. if XY matches, we are good.
                    continue; // none of the edges of that face match our curve so we can skip

                return f;
            }

            return null;
        }

        private static void GetGlazingFromCurtainWall(RVT.Wall wall, RVT.Face face, double tolerance, ref List<List<RVT.XYZ>> glazingPts, ref List<double> glazingAreas)
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
                        var perpendicular = face.ComputeNormal(new RVT.UV(0.5, 0.5)).CrossProduct(direction);
                        var offset = 0.1 * perpendicular;
                        var offsetPt = pt + offset;

                        hPts[i] = offsetPt;
                    }
                }

                if (hPts.Count < 3) continue;

                glazingAreas.Add(PolygonArea(hUvs));
                glazingPts.Add(hPts);
            }
        }

        private static double PolygonArea(IList<RVT.UV> polygon)
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

        private static bool GetHull(List<RVT.XYZ> pts, List<RVT.UV> uvs, double tolerance, out List<RVT.XYZ> hullPts, out List<RVT.UV> hullUvs)
        {
            hullPts = new List<RVT.XYZ>();
            hullUvs = new List<RVT.UV>();

            if (!pts.Any() || !uvs.Any())
                return false;

            try
            {
                var hullPoints = uvs.Select(x => new HullPoint(x.U, x.V)).ToList();
                var hull = ConvexHull.MakeHull(hullPoints);

                var hUvs = hull.Select(x => new RVT.UV(x.x, x.y)).ToList();
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
                    RVT.XYZ middle;
                    RVT.XYZ end;
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

                    var line = RVT.Line.CreateBound(start, end);
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

        private static bool GetPointsOnFace(RVT.Face face, List<RVT.XYZ> pts, out List<RVT.XYZ> ptsOnFace, out List<RVT.UV> uvsOnFace)
        {
            var onFace = new HashSet<RVT.XYZ>();
            var onFaceUvs = new HashSet<RVT.UV>();
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

        private static List<RVT.XYZ> GetGeometryPoints(RVT.Element e)
        {
            var pts = new List<RVT.XYZ>();
            using (var opt = new RVT.Options())
            {
                opt.IncludeNonVisibleObjects = true;
                using (var geom = e.get_Geometry(opt))
                {
                    ExtractPtsRecursively(geom, ref pts);
                }
            }

            return pts;
        }

        private static void ExtractPtsRecursively(RVT.GeometryElement geo, ref List<RVT.XYZ> pts, bool includeLines = false)
        {
            foreach (var g in geo)
            {
                var instGeo = g as RVT.GeometryInstance;
                if (instGeo != null)
                {
                    ExtractPtsRecursively(instGeo.GetInstanceGeometry(), ref pts, includeLines);
                    continue;
                }

                var solidGeo = g as RVT.Solid;
                if (solidGeo != null)
                {
                    foreach (RVT.Face f in solidGeo.Faces)
                    {
                        ProcessFace(f, ref pts);
                    }

                    continue;
                }

                var faceGeo = g as RVT.Face;
                if (faceGeo != null) ProcessFace(faceGeo, ref pts);

                var meshGeo = g as RVT.Mesh;
                if (meshGeo != null) pts.AddRange(meshGeo.Vertices);

                if (!includeLines) continue;

                var lineGeo = g as RVT.Curve;
                if (lineGeo != null && lineGeo.IsBound)
                    pts.AddRange(new List<RVT.XYZ> {lineGeo.GetEndPoint(0), lineGeo.GetEndPoint(1)});
            }
        }

        private static readonly double[] _params = {0d, 0.2, 0.4, 0.6, 0.8};

        private static void ProcessFace(RVT.Face f, ref List<RVT.XYZ> pts)
        {
            foreach (RVT.EdgeArray edges in f.EdgeLoops)
            {
                foreach (RVT.Edge e in edges)
                {
                    pts.AddRange(_params.Select(p => e.Evaluate(p)));
                }
            }
        }

        //private static List<List<Point2D>> GetHoles(IList<IList<RVT.BoundarySegment>> bs)
        //{
        //    var holes = new List<List<Point2D>>();
        //    for (var i = 1; i < bs.Count; i++)
        //    {
        //        // (Konrad) All loops starting from index 1 are floor holes.
        //        var holeBoundary = bs[i];
        //        holes.Add(GetPoints(holeBoundary));
        //    }

        //    return holes;
        //}

        //private static List<Point2D> GetBoundary(IEnumerable<IList<RVT.BoundarySegment>> bs)
        //{
        //    var boundary = new List<Point2D>();
        //    var outerBoundary = bs.FirstOrDefault();
        //    if (outerBoundary == null) return boundary;

        //    boundary.AddRange(GetPoints(outerBoundary));

        //    return boundary;
        //}

        //private static double GetCeilingHeight(RVT.SpatialElement se)
        //{
        //    try
        //    {
        //        var view = new RVT.FilteredElementCollector(se.Document)
        //            .OfClass(typeof(RVT.View3D))
        //            .Cast<RVT.View3D>()
        //            .FirstOrDefault(x => !x.IsTemplate);
        //        if (view == null)
        //            return se.GetUnboundHeight();

        //        var basePt = se.GetLocationPoint();
        //        if (basePt == null)
        //            return se.GetUnboundHeight();

        //        var direction = new RVT.XYZ(0, 0, 1);
        //        var filter = new RVT.ElementClassFilter(typeof(RVT.Ceiling));
        //        var refIntersector = new RVT.ReferenceIntersector(filter, RVT.FindReferenceTarget.Face, view);
        //        var refWithContext = refIntersector.FindNearest(basePt, direction);
        //        if (refWithContext == null)
        //        {
        //            // (Konrad) There is no Ceiling. What about a Floor (exposed ceiling)?
        //            basePt += new RVT.XYZ(0, 0, 0.1); // floor/bottom of room intersect let's move point up
        //            filter = new RVT.ElementClassFilter(typeof(RVT.Floor));
        //            refIntersector = new RVT.ReferenceIntersector(filter, RVT.FindReferenceTarget.Face, view);
        //            refWithContext = refIntersector.FindNearest(basePt, direction);
        //        }

        //        if (refWithContext == null)
        //        {
        //            // (Konrad) There is no Floor. What about Roof (exposed ceiling on top floor)?
        //            filter = new RVT.ElementClassFilter(typeof(RVT.RoofBase));
        //            refIntersector = new RVT.ReferenceIntersector(filter, RVT.FindReferenceTarget.Face, view);
        //            refWithContext = refIntersector.FindNearest(basePt, direction);
        //        }

        //        if (refWithContext == null)
        //            return se.GetUnboundHeight();

        //        var reference = refWithContext.GetReference();
        //        var intersection = reference.GlobalPoint;
        //        var line = RVT.Line.CreateBound(basePt, intersection);
        //        var height = line.Length;

        //        return height;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Fatal(e);
        //        return 0d;
        //    }
        //}

        //private static List<Point2D> GetPoints(IEnumerable<RVT.BoundarySegment> segments)
        //{
        //    var curves = new List<Point2D>();
        //    foreach (var bs in segments)
        //    {
        //        var curve = bs.GetCurve();
        //        switch (curve)
        //        {
        //            case RVT.Line line:
        //                curves.Add(new Point2D(line.GetEndPoint(0)));
        //                break;
        //            case RVT.Arc arc:
        //                curves.Add(new Point2D(arc.Evaluate(0, true)));
        //                curves.Add(new Point2D(arc.Evaluate(0.25, true)));
        //                curves.Add(new Point2D(arc.Evaluate(0.5, true)));
        //                curves.Add(new Point2D(arc.Evaluate(0.75, true)));
        //                break;
        //            case RVT.CylindricalHelix unused:
        //            case RVT.Ellipse unused1:
        //            case RVT.HermiteSpline unused2:
        //            case RVT.NurbSpline unused3:
        //                break;
        //        }
        //    }

        //    return curves;
        //}

        //private static List<WindowParameterBase> GetWindowParameters(IList<IList<RVT.BoundarySegment>> bs, RVT.Document doc)
        //{
        //    var wParams = new List<WindowParameterBase>();
        //    var outerBoundary = bs.FirstOrDefault();
        //    if (outerBoundary == null) return wParams;

        //    for (var i = 0; i < bs.Count; i++)
        //    {
        //        var loop = bs[i];
        //        for (var k = 0; k < loop.Count; k++)
        //        {
        //            var segment = bs[i][k];
        //            var bElement = doc.GetElement(segment.ElementId);
        //            if (bElement == null) continue;

        //            if (bElement is RVT.Wall wall)
        //            {
        //                var inserts = wall.FindInserts(true, false, true, true)
        //                    .Select(doc.GetElement)
        //                    .Where(x => x.Category.Id.IntegerValue == RVT.BuiltInCategory.OST_Windows.GetHashCode())
        //                    .ToList();
        //            }
        //        }
        //    }

        //    return wParams;
        //}

        #endregion
    }

    public static class RoomExtensions
    {
        public static List<HB.AnyOf<HB.Ground, HB.Outdoors, HB.Adiabatic, HB.Surface>> ToDragonfly(
            this List<BoundaryConditionBase> bcs)
        {
            var boundaryConditions = new List<HB.AnyOf<HB.Ground, HB.Outdoors, HB.Adiabatic, HB.Surface>>();
            foreach (var bc in bcs)
            {
                switch (bc)
                {
                    case Outdoors unused:
                        boundaryConditions.Add(bc.ToDragonfly() as HB.Outdoors);
                        break;
                    case Ground unused:
                        boundaryConditions.Add(bc.ToDragonfly() as HB.Ground);
                        break;
                    case Adiabatic unused:
                        boundaryConditions.Add(bc.ToDragonfly() as HB.Adiabatic);
                        break;
                    case Surface unused:
                        boundaryConditions.Add(bc.ToDragonfly() as HB.Surface);
                        break;
                    default:
                        boundaryConditions.Add(null);
                        break;
                }
            }

            return boundaryConditions;
        }

        public static List<HB.AnyOf<DF.SingleWindow, DF.SimpleWindowRatio, DF.RepeatingWindowRatio, DF.RectangularWindows, DF.DetailedWindows>> ToDragonfly(
            this List<WindowParameterBase> bcs)
        {
            var windowParameters = new List<HB.AnyOf<DF.SingleWindow, DF.SimpleWindowRatio, DF.RepeatingWindowRatio, DF.RectangularWindows, DF.DetailedWindows>>();
            foreach (var bc in bcs)
            {
                switch (bc)
                {
                    case SingleWindow unused:
                        windowParameters.Add(bc.ToDragonfly() as DF.SingleWindow);
                        break;
                    case SimpleWindowRatio unused:
                        windowParameters.Add(bc.ToDragonfly() as DF.SimpleWindowRatio);
                        break;
                    case RepeatingWindowRatio unused:
                        windowParameters.Add(bc.ToDragonfly() as DF.RepeatingWindowRatio);
                        break;
                    case RectangularWindows unused:
                        windowParameters.Add(bc.ToDragonfly() as DF.RectangularWindows);
                        break;
                    case DetailedWindows unused:
                        windowParameters.Add(bc.ToDragonfly() as DF.DetailedWindows);
                        break;
                    default:
                        windowParameters.Add(null);
                        break;
                }
            }

            return windowParameters;
        }
    }
}