using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class ModelProperties : ISchema<DF.ModelProperties>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public ModelEnergyProperties Energy { get; set; } = new ModelEnergyProperties();

        public DF.ModelProperties ToDragonfly()
        {
            return new DF.ModelProperties(Type);
        }
    }
}
