using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas.Honeybee
{
    public class Shade : IBaseObject, ISchema<DF.ContextShade, HB.Shade>
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("identifier")]
        public string Identifier { get; set; } = $"Shade_{Guid.NewGuid()}";

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("geometry")]
        public Face3D Geometry { get; set; }

        [JsonProperty("properties")]
        public ShadePropertiesAbridged Properties { get; set; } = new ShadePropertiesAbridged();

        public Shade(List<Point3D> boundary)
        {
            Geometry = new Face3D(boundary);
        }

        public Shade(Face3D face)
        {
            Geometry = face;
        }

        public DF.ContextShade ToDragonfly()
        {
            return new DF.ContextShade(
                Identifier,
                new List<HB.Face3D> {Geometry.ToHoneybee()},
                Properties.ToDragonfly(),
                DisplayName,
                null // user data
            );
        }

        public HB.Shade ToHoneybee()
        {
            return new HB.Shade(
                Identifier,
                Geometry.ToHoneybee(),
                Properties.ToHoneybee(),
                DisplayName,
                null // user data
            );
        }
    }
}
