using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas.Honeybee
{
    public class Aperture : IBaseObject, ISchema<object, HB.Aperture>
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; } = $"Aperture_{Guid.NewGuid()}";

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("geometry")]
        public Face3D Geometry { get; set; }

        [JsonProperty("boundary_condition")]
        public BoundaryConditionBase BoundaryCondition { get; set; }

        [JsonProperty("properties")]
        public HB.AperturePropertiesAbridged Properties { get; set; } = new HB.AperturePropertiesAbridged();

        [JsonProperty("is_operable")]
        public bool IsOperable { get; set; }

        [JsonProperty("indoor_shades")]
        public List<HB.Shade> IndoorShades { get; set; } = new List<HB.Shade>();

        [JsonProperty("outdoor_shades")]
        public List<HB.Shade> OutdoorShades { get; set; } = new List<HB.Shade>();

        public Aperture(List<Point3D> boundary)
        {
            Geometry = new Face3D(boundary);
        }

        public object ToDragonfly()
        {
            throw new NotImplementedException();
        }

        public HB.Aperture ToHoneybee()
        {
            return new HB.Aperture(
                Identifier,
                Geometry?.ToHoneybee(),
                BoundaryCondition.ToHoneybeeAnyOf(),
                Properties,
                DisplayName,
                null, // user data
                IsOperable,
                IndoorShades,
                OutdoorShades
            );
        }
    }

    public static class ApertureExtensions
    {
        public static HB.AnyOf<HB.Outdoors, HB.Surface> ToHoneybeeAnyOf(this BoundaryConditionBase bc)
        {
            switch (bc)
            {
                case Adiabatic unused:
                case Ground unused1:
                    return new HB.AnyOf<HB.Outdoors, HB.Surface>(new HB.Outdoors());
                case Outdoors outdoors:
                    return new HB.AnyOf<HB.Outdoors, HB.Surface>(outdoors.ToHoneybee() as HB.Outdoors);
                case Surface surface:
                    return new HB.AnyOf<HB.Outdoors, HB.Surface>(surface.ToHoneybee() as HB.Surface);
                default:
                    return new HB.AnyOf<HB.Outdoors, HB.Surface>(new HB.Outdoors());
            }
        }
    }
}
