using System;
using Newtonsoft.Json;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class ShadeConstruction : ConstructionBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("identifier")]
        public override string Identifier { get; set; } = $"ShadeConstruction_{Guid.NewGuid()}";

        [JsonProperty("display_name")]
        public override string DisplayName { get; set; }

        [JsonProperty("solar_reflectance")]
        public double SolarReflectance { get; set; } = 0.2d;

        [JsonProperty("visible_reflectance")]
        public double VisibleReflectance { get; set; } = 0.2d;

        [JsonProperty("is_specular")]
        public bool IsSpecular { get; set; }

        public override object ToDragonfly()
        {
            return new HB.ShadeConstruction(Identifier, SolarReflectance, VisibleReflectance, IsSpecular, DisplayName);
        }
    }
}
