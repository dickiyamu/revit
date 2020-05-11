using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Honeybee.Revit.Schemas.Converters
{
    public class BoundaryConditionBaseConverter : JsonConverter
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
                case "Ground":
                    return jo.ToObject<Ground>(serializer);
                case "Outdoors":
                    return jo.ToObject<Outdoors>(serializer);
                case "Surface":
                    return jo.ToObject<DragonflySurface>(serializer);
                case "Adiabatic":
                    return jo.ToObject<Adiabatic>(serializer);
                default:
                    return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BoundaryConditionBase);
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}
