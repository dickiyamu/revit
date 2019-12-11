using Newtonsoft.Json;

namespace Honeybee.Revit.Schemas
{
    public abstract class BoundaryCondition
    {
        [JsonProperty("type")]
        public abstract string Type { get; }
    }

    public class Outdoors : BoundaryCondition
    {
        public override string Type
        {
            get { return "Outdoors"; }
        }

        [JsonProperty("sun_exposure")]
        public bool SunExposure { get; set; }

        [JsonProperty("wind_exposure")]
        public bool WindExposure { get; set; }

        [JsonProperty("view_factor")]
        public string ViewFactor { get; set; } = "autocalculate";
    }

    public class Ground : BoundaryCondition
    {
        public override string Type
        {
            get { return "Ground"; }
        }
    }
}
