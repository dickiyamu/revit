using System;
using System.Collections.Generic;
using System.Windows.Documents;
using Newtonsoft.Json;

namespace Honeybee.Revit.Schemas.Converters
{
    public class Point2DConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Point2D pt) serializer.Serialize(writer, new List<double> {pt.X, pt.Y});
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
            return objectType == typeof(Point2D);
        }
    }
}
