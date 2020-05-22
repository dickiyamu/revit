using System;
using Autodesk.Revit.DB;
using Honeybee.Revit.Schemas.Converters;
using Honeybee.Revit.Utilities;
using Newtonsoft.Json;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Honeybee.Revit.Schemas
{
    [JsonConverter(typeof(Point3DConverter))]
    public class Point3D : IEquatable<Point3D>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        [JsonConstructor]
        public Point3D()
        {
        }

        public Point3D(XYZ pt)
        {
            X = pt.X;
            Y = pt.Y;
            Z = pt.Z;
        }

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public XYZ ToXyz()
        {
            return new XYZ(X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point3D other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public bool Equals(Point3D other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            return X.AlmostEqualTo(other.X)
                   && Y.AlmostEqualTo(other.Y)
                   && Z.AlmostEqualTo(other.Z);
        }
    }

    [JsonConverter(typeof(Point2DConverter))]
    public class Point2D : IEquatable<Point2D>
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

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point2D other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public bool Equals(Point2D other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            return X.AlmostEqualTo(other.X)
                   && Y.AlmostEqualTo(other.Y);
        }
    }
}
