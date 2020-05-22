using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class BuildingEnergyPropertiesAbridged : ISchema<DF.BuildingEnergyPropertiesAbridged, object>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public string ConstructionSet { get; set; }

        public DF.BuildingEnergyPropertiesAbridged ToDragonfly()
        {
            return new DF.BuildingEnergyPropertiesAbridged(ConstructionSet);
        }

        public object ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }
}
