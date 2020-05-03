using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Honeybee.Revit.Schemas
{
    public static class GeometryUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static List<Point2D> GetPoints(Curve curve)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static List<Point3D> GetPoints3D(Curve curve)
        {
            var pts = new List<Point3D>();
            switch (curve)
            {
                case Line line:
                    pts.Add(new Point3D(line.GetEndPoint(0)));
                    break;
                case Arc arc:
                    pts.Add(new Point3D(arc.Evaluate(0, true)));
                    pts.Add(new Point3D(arc.Evaluate(0.25, true)));
                    pts.Add(new Point3D(arc.Evaluate(0.5, true)));
                    pts.Add(new Point3D(arc.Evaluate(0.75, true)));
                    break;
                case CylindricalHelix unused:
                case Ellipse unused1:
                case HermiteSpline unused2:
                case NurbSpline unused3:
                    break;
            }

            return pts;
        }
    }
}
