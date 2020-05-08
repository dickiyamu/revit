using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class StoryEnergyPropertiesAbridged : ISchema<DF.StoryEnergyPropertiesAbridged, object>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public string ConstructionSet { get; set; }

        public DF.StoryEnergyPropertiesAbridged ToDragonfly()
        {
            return new DF.StoryEnergyPropertiesAbridged(ConstructionSet);
        }

        public object ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }
}
