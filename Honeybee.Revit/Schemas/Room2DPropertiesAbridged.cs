using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas
{
    public class Room2DPropertiesAbridged : ISchema<DF.Room2DPropertiesAbridged>
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        public Room2DEnergyPropertiesAbridged Energy { get; set; } = new Room2DEnergyPropertiesAbridged();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DF.Room2DPropertiesAbridged ToDragonfly()
        {
            return new DF.Room2DPropertiesAbridged(null);
        }
    }
}
