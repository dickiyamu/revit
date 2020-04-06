using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class Story : IBaseObject, ISchema<DF.Story>
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("name")]
        public string Name { get; set; } = $"Story_{Guid.NewGuid()}";

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

        public Story(string displayName, List<Room2D> rooms, StoryPropertiesAbridged properties)
        {
            DisplayName = displayName;
            Room2Ds = rooms;
            Properties = properties;
        }

        public DF.Story ToDragonfly()
        {
            return new DF.Story(
                Name,
                Room2Ds.Select(x => x.ToDragonfly()).ToList(),
                Properties.ToDragonfly(),
                DisplayName,
                Type,
                FloorToFloorHeight,
                Multiplier
            );
        }
    }
}
