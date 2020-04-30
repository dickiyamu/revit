using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class StoryEnergyPropertiesAbridged : ISchema<DF.StoryEnergyPropertiesAbridged>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public ConstructionSetAbridged ConstructionSet { get; set; }

        public DF.StoryEnergyPropertiesAbridged ToDragonfly()
        {
            return new DF.StoryEnergyPropertiesAbridged(Type, ConstructionSet?.Identifier);
        }
    }
}
