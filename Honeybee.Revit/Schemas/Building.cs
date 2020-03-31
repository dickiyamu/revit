using System.Collections.Generic;
using System.Linq;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class Building : ISchema<DF.Building>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public string Name { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public List<Story> UniqueStories { get; set; }
        public BuildingPropertiesAbridged Properties { get; set; }

        public Building(string name, List<Story> stories, BuildingPropertiesAbridged properties)
        {
            Name = name;
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