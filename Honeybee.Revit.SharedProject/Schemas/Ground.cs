using Newtonsoft.Json;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class Ground : BoundaryConditionBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        public override object ToDragonfly()
        {
            return new HB.Ground();
        }

        public override object ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }
}
