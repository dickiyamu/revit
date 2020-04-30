using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Honeybee.Core.Extensions;
using Honeybee.Revit.CreateModel.Wrappers;

namespace Honeybee.Revit.Schemas
{
    public abstract class BoundaryConditionBase : ISchema<object>
    {
        public abstract string Type { get; }
        public abstract object ToDragonfly();

        public static BoundaryConditionBase Init(IEnumerable<SpatialObjectWrapper> objects, Curve curve, SpatialObjectWrapper sow, bool allowAdiabatic)
        {
            var adjacentRoomName = string.Empty;
            var adjacentCurveIndex = -1;
            foreach (var so in objects)
            {
                if (adjacentCurveIndex != -1 && adjacentRoomName != null)
                    break;

                var boundarySegments = so.Room2D.FloorBoundary.GetCurves(so.Level.Elevation);
                for (var i = 0; i < boundarySegments.Count; i++)
                {
                    var c = boundarySegments[i];
                    if (!c.OverlapsWithIn2D(curve))
                        continue;

                    adjacentCurveIndex = i;
                    adjacentRoomName = so.Room2D.Identifier;
                    break;
                }
            }

            if (adjacentCurveIndex != -1 && !string.IsNullOrWhiteSpace(adjacentRoomName))
            {
                // (Konrad) We found a matching Surface Boundary Condition.
                var bConditionObj = new Tuple<int, string>(adjacentCurveIndex, adjacentRoomName);

                return new Surface(bConditionObj);
            }

            if (!allowAdiabatic)
                return new Outdoors();

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
            if (room != null && room.Id != sow.Self.Id)
                return new Adiabatic();

            var inPt = midPoint + perpendicular.Negate();
            room = phases.Cast<Phase>().Select(x => doc.GetRoomAtPoint(inPt, x)).FirstOrDefault(x => x != null);
            if (room != null && room.Id != sow.Self.Id)
                return new Adiabatic();

            // (Konrad) We can't find the Room adjacent to this Curve.
            return new Outdoors();
        }
    }
}
