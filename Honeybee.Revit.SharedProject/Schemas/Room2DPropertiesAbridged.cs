using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class Room2DPropertiesAbridged : ISchema<DF.Room2DPropertiesAbridged, HB.RoomPropertiesAbridged>
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
            return new DF.Room2DPropertiesAbridged(Energy.ToDragonfly());
        }

        public HB.RoomPropertiesAbridged ToHoneybee()
        {
            return new HB.RoomPropertiesAbridged(Energy.ToHoneybee());
        }
    }
}
