using System;
using Honeybee.Revit.Schemas.Converters;
using Newtonsoft.Json;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class Surface : BoundaryConditionBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("boundary_condition_objects")]
        [JsonConverter(typeof(BoundaryConditionObjectsConverter))]
        public Tuple<int, string> BoundaryConditionObjects { get; set; }

        [JsonConstructor]
        public Surface()
        {
        }

        public Surface(Tuple<int, string> bConObj)
        {
            BoundaryConditionObjects = bConObj;
        }

        public override object ToDragonfly()
        {
            return new HB.Surface(BoundaryConditionObjects.ToDragonfly());
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }
}
