using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Honeybee.Core.Extensions;
using Honeybee.Revit.CreateModel.Wrappers;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public abstract class BoundaryCondition : ISchema<object>
    {
        public abstract string Type { get; }
        public abstract object ToDragonfly();

        public static BoundaryCondition Init(IEnumerable<SpatialObjectWrapper> objects, Curve curve, SpatialObjectWrapper sow)
        {
            var adjacentRoomName = string.Empty;
            var adjacentCurveIndex = -1;
            foreach (var so in objects)
            {
                if (adjacentCurveIndex != -1 && adjacentRoomName != null) break;

                for (var i = 0; i < so.Room2D.FloorBoundarySegments.Count; i++)
                {
                    var c = so.Room2D.FloorBoundarySegments[i];
                    if (!c.OverlapsWith(curve)) continue;

                    adjacentCurveIndex = i;
                    adjacentRoomName = so.Room2D.Name;
                    break;
                }
            }

            if (adjacentCurveIndex != -1 && !string.IsNullOrWhiteSpace(adjacentRoomName))
            {
                // (Konrad) We found a matching Surface Boundary Condition.
                var bConditionObj = new Tuple<int, string>(adjacentCurveIndex, adjacentRoomName);
                return new Surface(bConditionObj);
            }

            // (Konrad) We can try assigning Adiabatic and Outdoors.
            var direction = curve is Line
                ? curve.ComputeDerivatives(0, true).BasisX.Normalize()
                : (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize();
            var perpendicular = XYZ.BasisZ.CrossProduct(direction).Multiply(2);

            var start = curve.GetEndPoint(0);
            var end = curve.GetEndPoint(1);
            var midPoint = new XYZ((start.X + end.X) / 2, (start.Y + end.Y) / 2, start.Z + 1);
            var outPt = midPoint + perpendicular;
            var doc = sow.Self.Document;
            var phases = doc.Phases;
            var room = phases.Cast<Phase>().Select(x => doc.GetRoomAtPoint(outPt, x)).FirstOrDefault(x => x != null);
            if (room != null && room.Id != sow.Self.Id) return new Adiabatic();

            var inPt = midPoint + perpendicular.Negate();
            room = phases.Cast<Phase>().Select(x => doc.GetRoomAtPoint(inPt, x)).FirstOrDefault(x => x != null);
            if (room != null && room.Id != sow.Self.Id) return new Adiabatic();

            // (Konrad) We can't find the Room adjacent to this Curve.
            return new Outdoors();
        }
    }

    public class Outdoors : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public bool SunExposure { get; set; }
        public bool WindExposure { get; set; }
        public string ViewFactor { get; set; } = "autocalculate";

        public override object ToDragonfly()
        {
            return new DF.Outdoors();
        }
    }

    public class Ground : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override object ToDragonfly()
        {
            return new DF.Ground();
        }
    }

    public class Adiabatic : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override object ToDragonfly()
        {
            return new DF.Adiabatic();
        }
    }

    public class Surface : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public Tuple<int, string> BoundaryConditionObjects { get; set; }

        public Surface(Tuple<int, string> bConObj)
        {
            BoundaryConditionObjects = bConObj;
        }

        public override object ToDragonfly()
        {
            return new DF.Surface(BoundaryConditionObjects.ToDragonfly(), Type);
        }
    }
}
