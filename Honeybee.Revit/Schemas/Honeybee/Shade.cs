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
        public HB.ShadePropertiesAbridged Properties { get; set; }

        public Shade(List<Point3D> boundary)
        {
            Geometry = new Face3D(boundary);
            Properties = new HB.ShadePropertiesAbridged();
        }

        public Shade(Face3D face)
        {
            Geometry = face;
            Properties = new HB.ShadePropertiesAbridged();
        }

        public DF.ContextShade ToDragonfly()
        {
            throw new NotImplementedException();
        }

        public HB.Shade ToHoneybee()
        {
            return new HB.Shade(
                Identifier,
                Geometry.ToHoneybee(),
                Properties,
                DisplayName,
                null // user data
            );
        }
    }
}
