using Newtonsoft.Json;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class Outdoors : BoundaryConditionBase
    {
        [JsonProperty("type")]
        public override string Type
        {
            get { return GetType().Name; }
        }

        [JsonProperty("sun_exposure")]
        public bool SunExposure { get; set; }

        [JsonProperty("wind_exposure")]
        public bool WindExposure { get; set; }

        [JsonProperty("view_factor")]
        public HB.AnyOf<HB.Autocalculate, double> ViewFactor { get; set; }

        public override object ToDragonfly()
        {
            return new HB.Outdoors();
        }

        public override object ToHoneybee()
        {
            return new HB.Outdoors();
        }
    }
}
