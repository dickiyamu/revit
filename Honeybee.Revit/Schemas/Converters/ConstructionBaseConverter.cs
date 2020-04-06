using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Honeybee.Revit.Schemas.Converters
{
    public class ConstructionBaseConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            switch (jo["type"].Value<string>())
            {
                case "OpaqueConstructionAbridged":
                    return jo.ToObject<OpaqueConstructionAbridged>(serializer);
                case "WindowConstructionAbridged":
                    return jo.ToObject<WindowConstructionAbridged>(serializer);
                case "ShadeConstruction":
                    return jo.ToObject<ShadeConstruction>(serializer);
                default:
                    return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ConstructionBase);
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}
