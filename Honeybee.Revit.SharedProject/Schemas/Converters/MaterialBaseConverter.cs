using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Honeybee.Revit.Schemas.Converters
{
    public class MaterialBaseConverter : JsonConverter
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
                case "EnergyMaterial":
                    return jo.ToObject<EnergyMaterial>(serializer);
                case "EnergyMaterialNoMass":
                    return jo.ToObject<EnergyMaterialNoMass>(serializer);
                case "EnergyWindowMaterialBlind":
                    return jo.ToObject<EnergyWindowMaterialBlind>(serializer);
                case "EnergyWindowMaterialGas":
                    return jo.ToObject<EnergyWindowMaterialGas>(serializer);
                case "EnergyWindowMaterialGasCustom":
                    return jo.ToObject<EnergyWindowMaterialGasCustom>(serializer);
                case "EnergyWindowMaterialGasMixture":
                    return jo.ToObject<EnergyWindowMaterialGasMixture>(serializer);
                case "EnergyWindowMaterialGlazing":
                    return jo.ToObject<EnergyWindowMaterialGlazing>(serializer);
                case "EnergyWindowMaterialShade":
                    return jo.ToObject<EnergyWindowMaterialShade>(serializer);
                case "EnergyWindowMaterialSimpleGlazSys":
                    return jo.ToObject<EnergyWindowMaterialSimpleGlazSys>(serializer);
                default:
                    return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MaterialBase);
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}
