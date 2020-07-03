using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using HB = HoneybeeSchema;
using RVT = Autodesk.Revit.DB;

namespace Honeybee.Revit.Schemas.Honeybee
{
    public class Face3D : ISchema<object, HB.Face3D>
    {
        [JsonProperty("boundary")]
        public List<Point3D> Boundary { get; set; }

        [JsonProperty("holes")]
        public List<List<Point3D>> Holes { get; set; }

        [JsonProperty("plane")]
        public HB.Plane Plane { get; set; }

        public Face3D()
        {

        }

        public Face3D(List<Point3D> boundary)
        {
            Boundary = boundary;
        }

        public Face3D(RVT.Face face, ref List<string> messages)
        {
            var tolerance = AppSettings.Instance.StoredSettings.GeometrySettings.Tolerance;
            var loops = new List<List<Point3D>>();
            var curveLoops = face.GetEdgesAsCurveLoops();

            foreach (var cLoop in curveLoops)
            {
                var loop = new List<Point3D>();
                foreach (var curve in cLoop)
                {
                    if (curve.Length < tolerance)
                        messages.Add($"Face contains a curve that is shorter than specified tolerance of {tolerance}.");

                    var pts = GeometryUtils.GetPoints3D(curve);
                    loop.AddRange(pts);
                }

                loops.Add(loop);
            }

            var j = 0;
            var maxArea = 0d;
            var maxIndex = -1;
            foreach (var polygon in loops)
            {
                GetPolygonPlane(polygon.Select(x => new RVT.XYZ(x.X, x.Y, x.Z)).ToList(), out _, out _, out var area);
                if (Math.Abs(maxArea) < Math.Abs(area))
                {
                    maxIndex = j;
                    maxArea = area;
                }
                j++;
            }

            Boundary = loops[maxIndex];
            loops.RemoveAt(maxIndex);
            Holes = loops;
        }

        public object ToDragonfly()
        {
            throw new NotImplementedException();
        }

        public HB.Face3D ToHoneybee()
        {
            return new HB.Face3D(
                Boundary?.ToHoneybee(),
                Holes?.ToHoneybee(),
                Plane
            );
        }

        #region Utilities

        private static void GetPolygonPlane(IReadOnlyList<RVT.XYZ> polygon, out RVT.XYZ normal, out double dist, out double area)
        {
            normal = RVT.XYZ.Zero;
            dist = area = 0.0;
            var n = polygon.Count;
            var rc = (2 < n);
            if (3 == n)
            {
                var a = polygon[0];
                var b = polygon[1];
                var c = polygon[2];
                var v = b - a;
                normal = v.CrossProduct(c - a);
                dist = normal.DotProduct(a);
            }
            else if (4 == n)
            {
                var a = polygon[0];
                var b = polygon[1];
                var c = polygon[2];
                var d = polygon[3];

                var normalX = (c.Y - a.Y) * (d.Z - b.Z) + (c.Z - a.Z) * (b.Y - d.Y);
                var normalY = (c.Z - a.Z) * (d.X - b.X) + (c.X - a.X) * (b.Z - d.Z);
                var normalZ = (c.X - a.X) * (d.Y - b.Y) + (c.Y - a.Y) * (b.X - d.X);
                normal = new RVT.XYZ(normalX, normalY, normalZ);

                dist = 0.25 * (normal.X * (a.X + b.X + c.X + d.X) + normal.Y * (a.Y + b.Y + c.Y + d.Y) + normal.Z * (a.Z + b.Z + c.Z + d.Z));
            }
            else if (4 < n)
            {
                RVT.XYZ a;
                var b = polygon[n - 2];
                var c = polygon[n - 1];
                var s = RVT.XYZ.Zero;

                for (var i = 0; i < n; ++i)
                {
                    a = b;
                    b = c;
                    c = polygon[i];

                    var normalX = normal.X + b.Y * (c.Z - a.Z);
                    var normalY = normal.Y + b.Z * (c.X - a.X);
                    var normalZ = normal.Z + b.X * (c.Y - a.Y);
                    normal = new RVT.XYZ(normalX, normalY, normalZ);

                    s += c;
                }
                dist = s.DotProduct(normal) / n;
            }
            if (rc)
            {
                //
                // the polygon area is half of the length 
                // of the non-normalized normal vector of the plane:
                //
                var length = normal.GetLength();
                rc = !IsZero(length);

                if (rc)
                {
                    normal /= length;
                    dist /= length;
                    area = 0.5 * length;
                }
            }
        }

        private static bool IsZero(double a)
        {
            const double eps = 1.0e-9;
            return eps > Math.Abs(a);
        }

        #endregion
    }

    public static class Extensions
    {
        public static List<List<double>> ToHoneybee(this List<Point3D> boundary)
        {
            return boundary.Select(x => new List<double> { x.X, x.Y, x.Z }).ToList();
        }

        public static List<List<List<double>>> ToHoneybee(this List<List<Point3D>> holes)
        {
            return holes.Select(x => x.Select(y => new List<double> { y.X, y.Y, y.Z }).ToList()).ToList();
        }
    }
}
