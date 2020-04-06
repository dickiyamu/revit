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

        [JsonProperty("name")]
        public string Name { get; set; } = $"Building_{Guid.NewGuid()}";

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("unique_stories")]
        public List<Story> UniqueStories { get; set; }

        [JsonProperty("properties")]
        public BuildingPropertiesAbridged Properties { get; set; }

        public Building(string displayName, List<Story> stories, BuildingPropertiesAbridged properties)
        {
            DisplayName = displayName;
            UniqueStories = stories;
            Properties = properties;
        }

        public DF.Building ToDragonfly()
        {
            return new DF.Building(
                Name,
                UniqueStories.Select(x => x.ToDragonfly()).ToList(),
                Properties.ToDragonfly(),
                DisplayName,
                Type
            );
        }
    }
}