using System;
using Autodesk.Revit.DB;

namespace Honeybee.Revit.Utilities
{
    public static class CurveExtensions
    {
        public static bool OverlapsWithIn2D(this Curve source, Curve bCurve)
        {
            var tolerance = AppSettings.Instance.StoredSettings.GeometrySettings.Tolerance;
            if (Math.Abs(source.Length - bCurve.Length) > tolerance)
                return false;

            var bCurveStart = bCurve.GetEndPoint(0).Flatten();
            var bCurveEnd = bCurve.GetEndPoint(1).Flatten();
            XYZ sourceStart;
            XYZ sourceEnd;

            var bCurveDir = (bCurve.GetEndPoint(1) - bCurve.GetEndPoint(0)).Normalize();
            var sourceDir = (source.GetEndPoint(1) - source.GetEndPoint(0)).Normalize();
            if (bCurveDir.IsAlmostEqualTo(sourceDir, tolerance))
            {
                sourceStart = source.GetEndPoint(0).Flatten();
                sourceEnd = source.GetEndPoint(1).Flatten();
            }
            else
            {
                sourceStart = source.GetEndPoint(1).Flatten();
                sourceEnd = source.GetEndPoint(0).Flatten();
            }

            return bCurveStart.DistanceTo(sourceStart) < tolerance && 
                   bCurveEnd.DistanceTo(sourceEnd) < tolerance;
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
