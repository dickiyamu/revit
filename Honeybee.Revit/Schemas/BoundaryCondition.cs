using System;
using System.Collections.Generic;
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
    }

    public class Outdoors : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override object ToDragonfly()
        {
            throw new NotImplementedException();
        }

        public bool SunExposure { get; set; }
        public bool WindExposure { get; set; }
        public string ViewFactor { get; set; } = "autocalculate";
    }

    public class Ground : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override object ToDragonfly()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }

    public class Surface : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public Tuple<int, string> BoundaryConditionObjects { get; set; }

        public Surface Init(IEnumerable<SpatialObjectWrapper> objects, Curve curve)
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

            if (adjacentCurveIndex == -1 || string.IsNullOrWhiteSpace(adjacentRoomName)) return null;

            BoundaryConditionObjects = new Tuple<int, string>(adjacentCurveIndex, adjacentRoomName);
            return this;
        }

        public override object ToDragonfly()
        {
            return new DF.Surface(BoundaryConditionObjects.ToDragonfly(), Type);
        }
    }
}
