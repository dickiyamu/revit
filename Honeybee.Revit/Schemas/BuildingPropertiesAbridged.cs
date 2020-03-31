using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class BuildingPropertiesAbridged : ISchema<DF.BuildingPropertiesAbridged>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public DF.BuildingPropertiesAbridged ToDragonfly()
        {
            return new DF.BuildingPropertiesAbridged();
        }
    }
}
