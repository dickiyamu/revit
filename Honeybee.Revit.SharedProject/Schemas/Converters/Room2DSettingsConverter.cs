using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Honeybee.Revit.Schemas.Converters
{
    public class Room2DSettingsConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var room = (Room2D)value;

            var o = new JObject
            {
                new JProperty(nameof(room.Identifier), new JValue(room.Identifier)),
                new JProperty(nameof(room.IsTopExposed), new JValue(room.IsTopExposed)),
                new JProperty(nameof(room.IsGroundContact), new JValue(room.IsGroundContact)),
                new JProperty(nameof(room.FloorToCeilingHeight), new JValue(room.FloorToCeilingHeight)),
                new JProperty(nameof(room.Properties.Energy.ConstructionSet), new JValue(room.Properties.Energy.ConstructionSet.Identifier)),
                new JProperty(nameof(room.Properties.Energy.ProgramType), new JValue(room.Properties.Energy.ProgramType.Identifier)),
            };

            o.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var o = JObject.Load(reader);
            var room = new Room2D();
            room.Identifier = o[nameof(room.Identifier)].Value<string>();
            room.IsTopExposed = o[nameof(room.IsTopExposed)].Value<bool>();
            room.IsGroundContact = o[nameof(room.IsGroundContact)].Value<bool>();
            room.FloorToCeilingHeight = o[nameof(room.FloorToCeilingHeight)].Value<double>();

            var constructionSetIdentifier = o[nameof(room.Properties.Energy.ConstructionSet)].Value<string>();
            room.Properties.Energy.ConstructionSet = new ConstructionSet(constructionSetIdentifier);

            var programTypeIdentifier = o[nameof(room.Properties.Energy.ProgramType)].Value<string>();
            room.Properties.Energy.ProgramType = new ProgramType(programTypeIdentifier);

            return room;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Room2D);
        }
    }
}
