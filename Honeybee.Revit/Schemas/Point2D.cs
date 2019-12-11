using Autodesk.Revit.DB;
using Honeybee.Revit.Schemas.Converters;
using Newtonsoft.Json;

namespace Honeybee.Revit.Schemas
{
    [JsonConverter(typeof(Point2DConverter))]
    public class Point2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        [JsonConstructor]
        public Point2D()
        {
        }

        public Point2D(XYZ pt)
        {
            X = pt.X;
            Y = pt.Y;
        }
    }
}
