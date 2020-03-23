using System;
using Newtonsoft.Json;
using DF = DragonflySchema;

namespace Honeybee.Revit.Schemas.Converters
{
    public class BoundaryConditionsConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface> anyOf) writer.WriteValue(anyOf.ToJson());
            else writer.WriteValue("");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DF.AnyOf<DF.Ground, DF.Outdoors, DF.Adiabatic, DF.Surface>);
        }
    }
}
