using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class Story : IBaseObject, ISchema<DF.Story, object>
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("identifier")]
        public string Identifier { get; set; } = $"Story_{Guid.NewGuid()}";

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("floor_to_floor_height")]
        public double FloorToFloorHeight { get; set; }

        [JsonProperty("multiplier")]
        public int Multiplier { get; set; } = 1;

        [JsonProperty("room_2ds")]
        public List<Room2D> Room2Ds { get; set; }

        [JsonProperty("properties")]
        public StoryPropertiesAbridged Properties { get; set; }

        [JsonProperty("user_data")]
        public object UserData { get; set; } = new Dictionary<string, object>();

        public Story(string displayName, List<Room2D> rooms, StoryPropertiesAbridged properties)
        {
            DisplayName = displayName;
            Room2Ds = rooms;
            Properties = properties;
        }

        public DF.Story ToDragonfly()
        {
            return new DF.Story(
                Identifier,
                Room2Ds.Select(x => x.ToDragonfly()).ToList(),
                Properties.ToDragonfly(),
                DisplayName,
                null, // user data
                FloorToFloorHeight,
                Multiplier
            );
        }

        public object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }
}
