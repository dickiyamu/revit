using Newtonsoft.Json;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class Adiabatic : BoundaryConditionBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override object ToDragonfly()
        {
            return new HB.Adiabatic();
        }

        public override object ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }
}
