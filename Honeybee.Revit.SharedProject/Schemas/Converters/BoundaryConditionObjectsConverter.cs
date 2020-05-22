using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Honeybee.Revit.Schemas.Converters
{
    public class BoundaryConditionObjectsConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var arr = JArray.Load(reader).ToObject<List<string>>();
            var parts = arr[0].Split(new[] { ".." }, StringSplitOptions.None);
            var adjacentRoomName = parts[0];
            var index = int.Parse(Regex.Match(parts[1], @"\d+").Value);

            return new Tuple<int, string>(index, adjacentRoomName);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Tuple<int, string>);
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}
