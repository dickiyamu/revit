using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Autodesk.Revit.DB;
using Honeybee.Revit.CreateModel;
using Honeybee.Revit.CreateModel.Wrappers;
using NLog;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class Room2D : ISchema<DF.Room2D>, INotifyPropertyChanged
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public string Type
        {
            get { return GetType().Name; }
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Room2DPropertiesAbridged Properties { get; set; } = new Room2DPropertiesAbridged();
        public List<Point2D> FloorBoundary { get; set; }
        public List<List<Point2D>> FloorHoles { get; set; }
        public double FloorHeight { get; set; }
        public double FloorToCeilingHeight { get; set; }
        public bool IsGroundContact { get; set; }
        public bool IsTopExposed { get; set; }
        public List<BoundaryCondition> BoundaryConditions { get; set; }
        public List<DF.AnyOf<DF.SingleWindow, DF.SimpleWindowRatio, DF.RepeatingWindowRatio, DF.RectangularWindows, DF.DetailedWindows>> WindowParameters { get; set; }
        public List<DF.AnyOf<DF.ExtrudedBorder, DF.Overhang, DF.LouversByDistance, DF.LouversByCount>> ShadingParameters { get; set; }

        internal List<Curve> FloorBoundarySegments { get; set; }
        internal List<AnnotationWrapper> Annotations { get; set; } = new List<AnnotationWrapper>();

        public Room2D(SpatialElement e)
        {
            Name = e.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
            DisplayName = e.Name;

            if (e.Document.GetElement(e.LevelId) is Level level)
                FloorHeight = level.Elevation;

            FloorToCeilingHeight = GetCeilingHeight(e);
            FloorBoundary = GetBoundary(e);
            FloorHoles = GetHoles(e);
            FloorBoundarySegments = GetBoundarySegments(e);
        }

        /// <summary>
        /// Converts Revit Room2D object into Dragonfly Schema compatible object.
        /// </summary>
        /// <returns>Dragonfly Schema Room2D.</returns>
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
                BoundaryConditions.ToDragonfly(),
                WindowParameters,
                ShadingParameters);
        }

        #region Utilities

        private static List<List<Point2D>> GetHoles(SpatialElement se)
        {
            var holes = new List<List<Point2D>>();
            var bOptions = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreCenter
            };

            var boundarySegments = se.GetBoundarySegments(bOptions);
            for (var i = 1; i < boundarySegments.Count; i++)
            {
                // (Konrad) All loops starting from index 1 are floor holes.
                var holeBoundary = boundarySegments[i];
                holes.Add(GetPoints(holeBoundary));
            }

            return holes;
        }

        private static List<Curve> GetBoundarySegments(SpatialElement se)
        {
            var segments = new List<Curve>();
            var bOptions = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreCenter
            };

            var outerBoundary = se.GetBoundarySegments(bOptions).FirstOrDefault();
            if (outerBoundary == null) return segments;

            segments.AddRange(outerBoundary.Select(bs => bs.GetCurve()));

            return segments;
        }

        private static List<Point2D> GetBoundary(SpatialElement se)
        {
            var boundary = new List<Point2D>();
            var bOptions = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.CoreCenter
            };

            var outerBoundary = se.GetBoundarySegments(bOptions).FirstOrDefault();
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
                    case CylindricalHelix helix:
                    case Ellipse ellipse:
                    case HermiteSpline hs:
                    case NurbSpline ns:
                        break;
                }
            }

            return curves;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
