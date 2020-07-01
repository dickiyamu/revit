using System;
using System.Collections.Generic;
using System.Linq;
using Honeybee.Revit.Schemas.Honeybee;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class Model : IBaseObject, ISchema<DF.Model, HB.Model>
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("identifier")]
        public string Identifier { get; set; } = $"Model_{Guid.NewGuid()}";

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("buildings")]
        public List<Building> Buildings { get; set; }

        [JsonProperty("properties")]
        public ModelProperties Properties { get; set; }

        [JsonProperty("context_shades")]
        public List<Shade> ContextShades { get; set; }

        [JsonProperty("north_angle")]
        public double NorthAngle { get; set; }

        [JsonProperty("units")]
        [JsonConverter(typeof(StringEnumConverter))]
        public HB.Units? Units { get; set; } = HB.Units.Meters;

        [JsonProperty("tolerance")]
        public double Tolerance { get; set; } = 0.0001d;

        [JsonProperty("angle_tolerance")]
        public double AngleTolerance { get; set; } = 1d;

        [JsonProperty("user_data")]
        public object UserData { get; set; } = new Dictionary<string, object>();

        public List<HB.Room> Rooms { get; set; } = new List<HB.Room>();

        public Model(string displayName, List<HB.Room> rooms, ModelProperties properties, List<Shade> shades)
        {
            DisplayName = displayName;
            Rooms = rooms;
            Properties = properties;
            ContextShades = shades;
        }

        public Model(string displayName, List<Building> buildings, ModelProperties properties, List<Shade> shades)
        {
            DisplayName = displayName;
            Buildings = buildings;
            Properties = properties;
            ContextShades = shades;
        }

        public DF.Model ToDragonfly()
        {
            return new DF.Model(
                Identifier,
                Buildings.Select(x => x.ToDragonfly()).ToList(),
                Properties.ToDragonfly(),
                DisplayName,
                null, // user data
                null, // version
                ContextShades.Select(x => x.ToDragonfly()).ToList(),
                Units,
                Tolerance,
                AngleTolerance
            );
        }

        public HB.Model ToHoneybee()
        {
            return new HB.Model(
                Identifier,
                Properties.ToHoneybee(),
                DisplayName,
                null, // user data
                null, //version
                Rooms,
                null, // orphaned faces
                ContextShades.Select(x => x.ToHoneybee()).ToList(),
                null, // orphaned apertures
                null, // orphaned doors
                Units,
                Tolerance,
                AngleTolerance
            );
        }
    }
}
