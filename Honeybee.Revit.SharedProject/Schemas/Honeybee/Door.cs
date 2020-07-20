using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas.Honeybee
{
    public class Door : IBaseObject, ISchema<object, HB.Door>
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; } = $"Door_{Guid.NewGuid()}";

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("geometry")]
        public Face3D Geometry { get; set; }

        [JsonProperty("boundary_condition")]
        public BoundaryConditionBase BoundaryCondition { get; set; }

        [JsonProperty("properties")]
        public HB.DoorPropertiesAbridged Properties { get; set; }

        [JsonProperty("is_glass")]
        public bool IsGlass { get; set; }

        [JsonProperty("indoor_shades")]
        public List<HB.Shade> IndoorShades { get; set; }

        [JsonProperty("outdoor_shades")]
        public List<HB.Shade> OutdoorShades { get; set; }

        [JsonProperty("user_data")]
        public object UserData { get; set; } = new Dictionary<string, object>();

        [JsonConstructor]
        public Door()
        {
        }

        public Door(List<Point3D> boundary)
        {
            Geometry = new Face3D { Boundary = boundary };
            BoundaryCondition = new Outdoors();
            Properties = new HB.DoorPropertiesAbridged();
        }

        public object ToDragonfly()
        {
            throw new NotImplementedException();
        }

        public HB.Door ToHoneybee()
        {
            return new HB.Door(
                Identifier,
                Geometry.ToHoneybee(),
                BoundaryCondition.ToHoneybeeAnyOf(),
                Properties,
                DisplayName,
                null, // user data
                IsGlass,
                IndoorShades,
                OutdoorShades
            );
        }
    }
}
