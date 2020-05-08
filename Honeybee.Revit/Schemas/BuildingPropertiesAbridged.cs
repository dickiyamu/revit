using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class BuildingPropertiesAbridged : ISchema<DF.BuildingPropertiesAbridged, object>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public BuildingEnergyPropertiesAbridged Energy { get; set; } = new BuildingEnergyPropertiesAbridged();

        public DF.BuildingPropertiesAbridged ToDragonfly()
        {
            return new DF.BuildingPropertiesAbridged(Energy.ToDragonfly());
        }

        public object ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }
}
