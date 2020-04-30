using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class Model : IBaseObject, ISchema<DF.Model>
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
        public List<DF.ContextShade> ContextShades { get; set; } = new List<DF.ContextShade>();

        [JsonProperty("north_angle")]
        public double NorthAngle { get; set; }

        [JsonProperty("units")]
        [JsonConverter(typeof(StringEnumConverter))]
        public DF.Model.UnitsEnum Units { get; set; } = DF.Model.UnitsEnum.Meters;

        [JsonProperty("tolerance")]
        public double Tolerance { get; set; } = 0.0001d;

        [JsonProperty("angle_tolerance")]
        public double AngleTolerance { get; set; } = 1d;

        [JsonProperty("user_data")]
        public object UserData { get; set; } = new Dictionary<string, object>();

        public Model(string displayName, List<Building> buildings, ModelProperties properties)
        {
            DisplayName = displayName;
            Buildings = buildings;
            Properties = properties;
        }

        public DF.Model ToDragonfly()
        {
            return new DF.Model(
                Identifier,
                Buildings.Select(x => x.ToDragonfly()).ToList(),
                Properties.ToDragonfly(),
                ContextShades,
                NorthAngle,
                Units,
                Tolerance,
                AngleTolerance,
                DisplayName,
                null // user data
            );
        }
    }
}
