using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var arr = JArray.Load(reader).ToObject<List<double>>();
            return new Point2D(arr[0], arr[1]);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Point2D);
        }
    }

    public class Point3DConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Point3D pt) serializer.Serialize(writer, new List<double> { pt.X, pt.Y, pt.Z });
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var arr = JArray.Load(reader).ToObject<List<double>>();
            return new Point3D(arr[0], arr[1], arr[2]);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Point3D);
        }
    }
}
