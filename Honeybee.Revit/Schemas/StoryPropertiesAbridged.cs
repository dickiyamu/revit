using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class StoryPropertiesAbridged : ISchema<DF.StoryPropertiesAbridged, object>
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

        public object ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }
}
