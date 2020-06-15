using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class ShadePropertiesAbridged : ISchema<DF.ContextShadePropertiesAbridged, HB.ShadePropertiesAbridged>
    {
        public ShadeEnergyProperties Energy { get; set; } = new ShadeEnergyProperties();

        public DF.ContextShadePropertiesAbridged ToDragonfly()
        {
            return new DF.ContextShadePropertiesAbridged(Energy.ToDragonfly());
        }

        public HB.ShadePropertiesAbridged ToHoneybee()
        {
            return new HB.ShadePropertiesAbridged();
        }
    }
}
