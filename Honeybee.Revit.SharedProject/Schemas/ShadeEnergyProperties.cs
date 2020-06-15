using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class ShadeEnergyProperties : ISchema<DF.ContextShadeEnergyPropertiesAbridged, HB.ShadeEnergyPropertiesAbridged>
    {
        public DF.ContextShadeEnergyPropertiesAbridged ToDragonfly()
        {
            return new DF.ContextShadeEnergyPropertiesAbridged();
        }

        public HB.ShadeEnergyPropertiesAbridged ToHoneybee()
        {
            return new HB.ShadeEnergyPropertiesAbridged();
        }
    }
}
