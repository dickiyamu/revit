using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class ModelProperties : ISchema<DF.ModelProperties, HB.ModelProperties>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public ModelEnergyProperties Energy { get; set; } = new ModelEnergyProperties();

        public DF.ModelProperties ToDragonfly()
        {
            return new DF.ModelProperties(Energy.ToDragonfly());
        }

        public HB.ModelProperties ToHoneybee()
        {
            return new HB.ModelProperties(Energy.ToHoneybee());
        }
    }
}
