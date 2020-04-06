using System;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class ShadeConstruction : ConstructionBase
    {
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override string Name { get; set; } = $"ShadeConstruction_{Guid.NewGuid()}";
        public double SolarReflectance { get; set; } = 0.2d;
        public double VisibleReflectance { get; set; } = 0.2d;
        public bool IsSpecular { get; set; }

        public override object ToDragonfly()
        {
            return new DF.ShadeConstruction(Name, Type, SolarReflectance, VisibleReflectance, IsSpecular);
        }
    }
}
