using System;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class ConstructionSetAbridged : ConstructionSetBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Identifier { get; set; } = $"ConstructionSetAbridged_{Guid.NewGuid()}";
        public override string DisplayName { get; set; }
        public HB.WallConstructionSetAbridged WallSet { get; set; } = new HB.WallConstructionSetAbridged();
        public HB.FloorConstructionSetAbridged FloorSet { get; set; } = new HB.FloorConstructionSetAbridged();
        public HB.RoofCeilingConstructionSetAbridged RoofCeilingSet { get; set; } = new HB.RoofCeilingConstructionSetAbridged();
        public HB.ApertureConstructionSetAbridged ApertureSet { get; set; } = new HB.ApertureConstructionSetAbridged();
        public HB.DoorConstructionSetAbridged DoorSet { get; set; } = new HB.DoorConstructionSetAbridged();
        public string ShadeConstruction { get; set; }
        public string AirBoundaryConstruction { get; set; }

        public override object ToDragonfly()
        {
            return new HB.ConstructionSetAbridged(
                Identifier,
                WallSet,
                FloorSet,
                RoofCeilingSet,
                ApertureSet,
                DoorSet,
                ShadeConstruction,
                AirBoundaryConstruction,
                DisplayName
            );
        }
    }
}