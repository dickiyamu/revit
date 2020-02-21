using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.Revit.DB;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class Room2D : ISchema<DF.Room2D>, INotifyPropertyChanged
    {
        public string Type
        {
            get { return GetType().Name; }
        }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Room2DPropertiesAbridged Properties { get; set; } = new Room2DPropertiesAbridged();
        public List<Point2D> FloorBoundary { get; set; } = new List<Point2D>();
        public List<List<Point2D>> FloorHoles { get; set; } = new List<List<Point2D>>();
        public double FloorHeight { get; set; }
        public double FloorToCeilingHeight { get; set; }

        //[JsonProperty("boundary_conditions")]
        //public List<BoundaryCondition> BoundaryConditions { get; set; }

        public bool IsGroundContact { get; set; }
        public bool IsTopExposed { get; set; }
        public List<DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface>> BoundaryConditions { get; set; }
        public List<DF.AnyOf<DF.SingleWindow, DF.SimpleWindowRatio, DF.RepeatingWindowRatio, DF.RectangularWindows, DF.DetailedWindows>> WindowParameters { get; set; }
        public List<DF.AnyOf<DF.ExtrudedBorder, DF.Overhang, DF.LouversByDistance, DF.LouversByCount>> ShadingParameters { get; set; }

        public Room2D(SpatialElement e)
        {
            Name = e.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
            DisplayName = e.Name;

            if (e.Document.GetElement(e.LevelId) is Level level)
                FloorHeight = level.Elevation;

            var bOptions = new SpatialElementBoundaryOptions
            {
                SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish
            };

            // (Konrad) We are only dealing with placed Rooms so there should always be at least one boundary list.
            // (Konrad) Also, Revit forces all Boundary Loops to be closed. That means that start point will be other line's end point.
            var boundarySegments = e.GetBoundarySegments(bOptions);
            for (var i = 0; i < boundarySegments.Count; i++)
            {
                if (i == 0)
                {
                    // (Konrad) First loop of segments goes into FloorBoundaries.
                    var outerBoundary = boundarySegments[i];
                    FloorBoundary.AddRange(GetPoints(outerBoundary));
                }
                else
                {
                    // (Konrad) All subsequent loops are FloorHoles.
                    var holeBoundary = boundarySegments[i];
                    FloorHoles.Add(GetPoints(holeBoundary));
                }
            }
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
                new List<List<List<double>>>(),
                IsGroundContact,
                IsTopExposed,
                BoundaryConditions,
                WindowParameters,
                ShadingParameters);
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
