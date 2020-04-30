using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Honeybee.Revit.Schemas.Converters
{
    public class WindowParameterBaseConverter : JsonConverter
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
                case "SingleWindow":
                    return jo.ToObject<SingleWindow>(serializer);
                case "SimpleWindowRatio":
                    return jo.ToObject<SimpleWindowRatio>(serializer);
                case "RepeatingWindowRatio":
                    return jo.ToObject<RepeatingWindowRatio>(serializer);
                case "RectangularWindows":
                    return jo.ToObject<RectangularWindows>(serializer);
                case "DetailedWindows":
                    return jo.ToObject<DetailedWindows>(serializer);
                default:
                    return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(WindowParameterBase);
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}
