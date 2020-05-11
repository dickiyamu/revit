using System;
using System.Collections.Generic;
using System.Linq;
using Honeybee.Core.Extensions;
using Honeybee.Revit.CreateModel.Wrappers;
using Honeybee.Revit.Schemas.Honeybee;
using RVT = Autodesk.Revit.DB;

namespace Honeybee.Revit.Schemas
{
    public abstract class BoundaryConditionBase : ISchema<object, object>
    {
        public abstract string Type { get; }
        public abstract object ToDragonfly();
        public abstract object ToHoneybee();

        public static BoundaryConditionBase HB_Init(
            IEnumerable<SpatialObjectWrapper> objects, 
            Face face, 
            SpatialObjectWrapper sow, 
            bool allowAdiabatic)
        {
            var adjacentRoomId = string.Empty;
            var adjacentFaceId = string.Empty;

            Face adjacentFace;
            foreach (var so in objects)
            {
                if (!string.IsNullOrWhiteSpace(adjacentRoomId) && 
                    !string.IsNullOrWhiteSpace(adjacentFaceId))
                    break;

                var faces = so.Room2D.Faces;
                for (var i = 0; i < faces.Count; i++)
                {
                    var f = faces[i];
                    if (!f.OverlapsWith(face))
                        continue;

                    adjacentFace = faces[i];
                    adjacentFaceId = faces[i].Identifier;
                    adjacentRoomId = so.Room2D.Identifier;
                    break;
                }
            }

            if (!string.IsNullOrWhiteSpace(adjacentRoomId) && 
                !string.IsNullOrWhiteSpace(adjacentFaceId))
            {
                // (Konrad) We found a matching Surface Boundary Condition.
                // Tuple for HB Surface is always ApertureId, FaceId, RoomId.
                var bConditionObj = new Tuple<string, string, string>(string.Empty, adjacentFaceId, adjacentRoomId);

                return new HoneybeeSurface(bConditionObj);
            }

            if (!allowAdiabatic)
                return new Outdoors();

            //var boundary = face.Geometry.Boundary;
            //var plane = RVT.Plane.CreateByThreePoints(boundary[0].ToXyz(), boundary[1].ToXyz(),
            //    boundary[boundary.Count - 1].ToXyz());
            //var perpendicular = plane.Normal.Multiply(2);

            // (Konrad) We can't find the Room adjacent to this Curve.
            return new Outdoors();
        }

        public static BoundaryConditionBase DF_Init(IEnumerable<SpatialObjectWrapper> objects, RVT.Curve curve, SpatialObjectWrapper sow, bool allowAdiabatic)
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

                return new DragonflySurface(bConditionObj);
            }

            if (!allowAdiabatic)
                return new Outdoors();

            // (Konrad) We can try assigning Adiabatic and Outdoors.
            var direction = curve is RVT.Line
                ? curve.ComputeDerivatives(0, true).BasisX.Normalize()
                : (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize();
            var perpendicular = RVT.XYZ.BasisZ.CrossProduct(direction).Multiply(2);

            var start = curve.GetEndPoint(0);
            var end = curve.GetEndPoint(1);
            var midPoint = new RVT.XYZ((start.X + end.X) / 2, (start.Y + end.Y) / 2, start.Z + 1);
            var outPt = midPoint + perpendicular;
            var doc = sow.Self.Document;
            var phases = doc.Phases;
            var room = phases.Cast<RVT.Phase>().Select(x => doc.GetRoomAtPoint(outPt, x)).FirstOrDefault(x => x != null);
            if (room != null && room.Id != sow.Self.Id)
                return new Adiabatic();

            var inPt = midPoint + perpendicular.Negate();
            room = phases.Cast<RVT.Phase>().Select(x => doc.GetRoomAtPoint(inPt, x)).FirstOrDefault(x => x != null);
            if (room != null && room.Id != sow.Self.Id)
                return new Adiabatic();

            // (Konrad) We can't find the Room adjacent to this Curve.
            return new Outdoors();
        }
    }
}
