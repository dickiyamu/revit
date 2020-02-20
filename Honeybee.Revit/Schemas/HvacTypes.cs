using System.Collections.Generic;
using Newtonsoft.Json;

namespace Honeybee.Revit.Schemas
{
    public abstract class HvacTypes
    {
        [JsonProperty("type")]
        public abstract string Type { get; }

        [JsonProperty("name")]
        public abstract string Name { get; }

        [JsonProperty("economizer_type")]
        public string EconomizerType { get; set; }

        [JsonProperty("demand_controlled_ventilation")]
        public bool DemandControlledVentilation { get; set; }

        [JsonProperty("sensible_heat_recovery")]
        public double SensibleHeatRecovery { get; set; }

        [JsonProperty("latent_heat_recovery")]
        public double LatentHeatRecovery { get; set; }

        [JsonProperty("heating_air_temperature")]
        public double HeatingAirTemperature { get; set; }

        [JsonProperty("cooling_air_temperature")]
        public double CoolingAirTemperature { get; set; }

        [JsonProperty("heating_limit")]
        public HeatingLimit HeatingLimit { get; set; }

        [JsonProperty("cooling_limit")]
        public CoolingLimit CoolingLimit { get; set; }

        public static IEnumerable<HvacTypes> GetAll()
        {
            return new List<HvacTypes>
            {
                new NullAirSystem(),
                new IdealAirSystemAbridged()
            };
        }
    }

    public class IdealAirSystemAbridged : HvacTypes
    {
        public override string Type
        {
            get { return "IdealAirSystemAbridged"; }
        }

        public override string Name
        {
            get { return "Default HVAC System"; }
        }

        [JsonConstructor]
        public IdealAirSystemAbridged()
        {
            EconomizerType = "DifferentialDryBulb";
            DemandControlledVentilation = false;
            SensibleHeatRecovery = 0d;
            LatentHeatRecovery = 0d;
            HeatingAirTemperature = 50d;
            CoolingAirTemperature = 13d;
            HeatingLimit = new HeatingLimit();
            CoolingLimit = new CoolingLimit();
        }
    }

    public class NullAirSystem : HvacTypes
    {
        public override string Type
        {
            get { return "<None>"; }
        }

        public override string Name
        {
            get { return "<None>"; }
        }
    }

    public class HeatingLimit
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return "Autosize"; }
        }
    }

    public class CoolingLimit
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return "Autosize"; }
        }
    }
}
