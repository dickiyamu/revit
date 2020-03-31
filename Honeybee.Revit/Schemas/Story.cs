using System.Collections.Generic;
using System.Linq;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class Story : ISchema<DF.Story>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public string Name { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public double FloorToFloorHeight { get; set; }
        public int Multiplier { get; set; }
        public List<Room2D> Room2Ds { get; set; }
        public StoryPropertiesAbridged Properties { get; set; }

        public Story(string name, List<Room2D> rooms, StoryPropertiesAbridged properties)
        {
            Name = name;
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
