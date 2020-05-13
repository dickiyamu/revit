using System;
using Autodesk.Revit.DB;

namespace Honeybee.Revit.Utilities
{
    public static class CurveExtensions
    {
        public static bool OverlapsWithIn2D(this Curve source, Curve compareTo)
        {
            var tolerance = AppSettings.Instance.StoredSettings.GeometrySettings.Tolerance;
            if (Math.Abs(source.Length - compareTo.Length) > tolerance) return false;

            var sourceStart = source.GetEndPoint(0).Flatten();
            var sourceEnd = source.GetEndPoint(1).Flatten();
            var compareToStart = compareTo.GetEndPoint(0).Flatten();
            var compareToEnd = compareTo.GetEndPoint(1).Flatten();

            return (sourceStart.IsAlmostEqualTo(compareToStart, tolerance) || sourceStart.IsAlmostEqualTo(compareToEnd, tolerance)) &&
                   (sourceEnd.IsAlmostEqualTo(compareToStart, tolerance) || sourceEnd.IsAlmostEqualTo(compareToEnd, tolerance));
        }

        public static Curve Offset(this Curve curve, double offset)
        {
            switch (curve)
            {
                case Line line:
                    var start = line.GetEndPoint(0);
                    var end = line.GetEndPoint(1);

                    var oStart = new XYZ(start.X, start.Y, start.Z + offset);
                    var oEnd = new XYZ(end.X, end.Y, end.Z + offset);

                    return Line.CreateBound(oStart, oEnd);
                case Arc arc:
                    var start1 = arc.Evaluate(0, true);
                    var mid = arc.Evaluate(0.5, true);
                    var end1 = arc.Evaluate(1, true);

                    var oStart1 = new XYZ(start1.X, start1.Y, start1.Z + offset);
                    var oMid = new XYZ(mid.X, mid.Y, mid.Z + offset);
                    var oEnd1 = new XYZ(end1.X, end1.Y, end1.Z + offset);

                    return Arc.Create(oStart1, oEnd1, oMid);
                case CylindricalHelix unused:
                case Ellipse unused1:
                case HermiteSpline unused2:
                case NurbSpline unused3:
                    return curve;
                default:
                    return curve;
            }
        }

        public static XYZ Flatten(this XYZ pt)
        {
            return new XYZ(pt.X, pt.Y, 0);
        }
    }
}
