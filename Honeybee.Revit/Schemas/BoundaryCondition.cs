using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Honeybee.Core.Extensions;
using Honeybee.Revit.CreateModel.Wrappers;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public abstract class BoundaryCondition : ISchema<DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface>>
    {
        public abstract string Type { get; }

        public abstract DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface> ToDragonfly();
    }

    public class Outdoors : BoundaryCondition
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface> ToDragonfly()
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

        public override DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface> ToDragonfly()
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

        public override DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface> ToDragonfly()
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

        public Tuple<Curve, Room2D> BoundaryConditionObjects { get; set; }

        public Surface(IEnumerable<SpatialObjectWrapper> objects, Curve curve)
        {
            Curve adjacentCurve = null;
            Room2D adjacentRoom = null;
            foreach (var so in objects)
            {
                if (adjacentCurve != null && adjacentRoom != null) break;

                foreach (var c in so.Room2D.FloorBoundarySegments)
                {
                    if (!c.OverlapsWith(curve)) continue;

                    adjacentCurve = c;
                    adjacentRoom = so.Room2D;
                    break;
                }
            }

            BoundaryConditionObjects = new Tuple<Curve, Room2D>(adjacentCurve, adjacentRoom);
        }

        public override DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface> ToDragonfly()
        {
            throw new NotImplementedException();
        }
    }
}
