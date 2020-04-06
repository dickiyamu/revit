using System;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class ConstructionSetAbridged : ISchema<DF.ConstructionSetAbridged>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public string Name { get; set; } = $"ConstructionSetAbridged_{Guid.NewGuid()}";
        public DF.WallSetAbridged WallSet { get; set; } = new DF.WallSetAbridged();
        public DF.FloorSetAbridged FloorSet { get; set; } = new DF.FloorSetAbridged();
        public DF.RoofCeilingSetAbridged RoofCeilingSet { get; set; } = new DF.RoofCeilingSetAbridged();
        public DF.ApertureSetAbridged ApertureSet { get; set; } = new DF.ApertureSetAbridged();
        public DF.DoorSetAbridged DoorSet { get; set; } = new DF.DoorSetAbridged();
        public string ShadeConstruction { get; set; }
        public string AirBoundaryConstruction { get; set; }

        public DF.ConstructionSetAbridged ToDragonfly()
        {
            return new DF.ConstructionSetAbridged(
                Name,
                Type,
                WallSet,
                FloorSet,
                RoofCeilingSet,
                ApertureSet,
                DoorSet,
                ShadeConstruction,
                AirBoundaryConstruction
            );
        }
    }
}