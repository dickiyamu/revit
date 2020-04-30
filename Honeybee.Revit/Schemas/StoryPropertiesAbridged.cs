using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class StoryPropertiesAbridged : ISchema<DF.StoryPropertiesAbridged>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public StoryEnergyPropertiesAbridged Energy { get; set; } = new StoryEnergyPropertiesAbridged();

        public DF.StoryPropertiesAbridged ToDragonfly()
        {
            return new DF.StoryPropertiesAbridged();
        }
    }
}
