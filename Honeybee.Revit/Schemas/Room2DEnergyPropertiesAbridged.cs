using Honeybee.Revit.Schemas.Converters;
using Newtonsoft.Json;

namespace Honeybee.Revit.Schemas
{
    public class Room2DEnergyPropertiesAbridged
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return GetType().Name; }
        }

        [JsonConverter(typeof(ConstructionSetConverter))]
        [JsonProperty("program_type")]
        public ProgramType ProgramType { get; set; }

        [JsonConverter(typeof(ConstructionSetConverter))]
        [JsonProperty("construction_set")]
        public ConstructionSet ConstructionSet { get; set; }
    }
}
