using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class ShadePropertiesAbridged : ISchema<DF.ContextShadePropertiesAbridged, HB.ShadePropertiesAbridged>
    {
        public DF.ContextShadePropertiesAbridged ToDragonfly()
        {
            return new DF.ContextShadePropertiesAbridged();
        }

        public HB.ShadePropertiesAbridged ToHoneybee()
        {
            return new HB.ShadePropertiesAbridged();
        }
    }
}
