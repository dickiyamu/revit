using System;
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

        private bool Autocalculate { get; } = true;
        private double ViewFactor { get; } = 0d;

        public override object ToDragonfly()
        {
            object obj;
            if (Autocalculate && Math.Abs(ViewFactor) < 0.001)
                obj = new HB.Autocalculate();
            else
                obj = ViewFactor;

            return new HB.Outdoors(SunExposure, WindExposure, new HB.AnyOf<HB.Autocalculate, double>(obj));
        }

        public override object ToHoneybee()
        {
            throw new NotImplementedException();
        }
    }
}
