using Newtonsoft.Json;

namespace Honeybee.Revit.Schemas
{
    public class Room2DPropertiesAbridged
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("energy")]
        public Room2DEnergyPropertiesAbridged Energy { get; set; }
    }
}
