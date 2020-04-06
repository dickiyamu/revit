#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
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

        [JsonProperty("name")]
        public string Name { get; set; } = $"Room_{Guid.NewGuid()}";

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

        [JsonProperty("is_ground_contact")]
        public bool IsGroundContact { get; set; }

        [JsonProperty("is_top_exposed")]
        public bool IsTopExposed { get; set; }

        [JsonProperty("boundary_conditions")]
        public List<BoundaryConditionBase> BoundaryConditions { get; set; } = new List<BoundaryConditionBase>();
        //public List<DF.AnyOf<DF.SingleWindow, DF.SimpleWindowRatio, DF.RepeatingWindowRatio, DF.RectangularWindows, DF.DetailedWindows>> WindowParameters { get; set; }
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

            var bOptions = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreCenter
            };
            var boundarySegments = e.GetBoundarySegments(bOptions);

            FloorToCeilingHeight = GetCeilingHeight(e);
            FloorBoundary = GetBoundary(boundarySegments);
            FloorHoles = GetHoles(boundarySegments);
        }

        public DF.Room2D ToDragonfly()
        {
            return new DF.Room2D(
                Name,
                FloorBoundary.ToDragonfly(),
                FloorHeight,
                FloorToCeilingHeight,
                Properties.ToDragonfly(),
                DisplayName,
                Type,
                FloorHoles.ToDragonfly(),
                IsGroundContact,
                IsTopExposed,
                BoundaryConditions.ToDragonfly()
                );
        }

        public override bool Equals(object obj)
        {
            return obj is Room2D item && Name.Equals(item.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #region Utilities

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

        #endregion
    }
}
