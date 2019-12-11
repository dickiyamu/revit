using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Newtonsoft.Json;

namespace Honeybee.Revit.Schemas
{
    public class Room2D
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("properties")]
        public Room2DPropertiesAbridged Properties { get; set; } = new Room2DPropertiesAbridged();

        [JsonProperty("floor_boundary")]
        public List<Point2D> FloorBoundary { get; set; } = new List<Point2D>();

        [JsonProperty("floor_height")]
        public double FloorHeight { get; set; }

        [JsonProperty("floor_to_ceiling_height")]
        public double FloorToCeilingHeight { get; set; }

        [JsonProperty("boundary_conditions")]
        public List<BoundaryCondition> BoundaryConditions { get; set; }

        [JsonConstructor]
        public Room2D()
        {
        }

        public Room2D(Room e)
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
            foreach (var bs in e.GetBoundarySegments(bOptions).First())
            {
                var curve = bs.GetCurve();
                switch (curve)
                {
                    case Line line:
                        FloorBoundary.Add(new Point2D(line.GetEndPoint(0)));
                        break;
                    case Arc arc:
                        FloorBoundary.Add(new Point2D(arc.Evaluate(0, true)));
                        FloorBoundary.Add(new Point2D(arc.Evaluate(0.25, true)));
                        FloorBoundary.Add(new Point2D(arc.Evaluate(0.5, true)));
                        FloorBoundary.Add(new Point2D(arc.Evaluate(0.75, true)));
                        break;
                    case CylindricalHelix helix:
                    case Ellipse ellipse:
                    case HermiteSpline hs:
                    case NurbSpline ns:
                        break;
                }
            }
        }

        public Room2D(Space e)
        {
            Name = e.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
            DisplayName = e.Name;

            if (!(e.Document.GetElement(e.LevelId) is Level level)) return;

            FloorHeight = level.Elevation;
        }
    }
}
