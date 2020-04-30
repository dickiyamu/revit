using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class Building : IBaseObject, ISchema<DF.Building>
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("identifier")]
        public string Identifier { get; set; } = $"Building_{Guid.NewGuid()}";

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("unique_stories")]
        public List<Story> UniqueStories { get; set; }

        [JsonProperty("properties")]
        public BuildingPropertiesAbridged Properties { get; set; }

        [JsonProperty("user_data")]
        public object UserData { get; set; } = new Dictionary<string, object>();

        public Building(string displayName, List<Story> stories, BuildingPropertiesAbridged properties)
        {
            DisplayName = displayName;
            UniqueStories = stories;
            Properties = properties;
        }

        public DF.Building ToDragonfly()
        {
            return new DF.Building(
                Identifier,
                UniqueStories.Select(x => x.ToDragonfly()).ToList(),
                Properties.ToDragonfly(),
                DisplayName,
                null,
                Type
            );
        }
    }
}