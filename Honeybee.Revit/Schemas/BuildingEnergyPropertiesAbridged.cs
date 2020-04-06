using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class BuildingEnergyPropertiesAbridged : ISchema<DF.BuildingEnergyPropertiesAbridged>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public string ConstructionSet { get; set; }

        public DF.BuildingEnergyPropertiesAbridged ToDragonfly()
        {
            return new DF.BuildingEnergyPropertiesAbridged(Type, ConstructionSet);
        }
    }
}
