using System.Collections.Generic;
using Newtonsoft.Json;

namespace Honeybee.Revit.Schemas
{
    public abstract class HvacTypes
    {
        public abstract string Type { get; }
        public abstract string Name { get; }
        public string EconomizerType { get; set; }
        public bool DemandControlledVentilation { get; set; }
        public double SensibleHeatRecovery { get; set; }
        public double LatentHeatRecovery { get; set; }
        public double HeatingAirTemperature { get; set; }
        public double CoolingAirTemperature { get; set; }
        public HeatingLimit HeatingLimit { get; set; }
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
            get { return GetType().Name; }
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
            get { return GetType().Name; }
        }

        public override string Name
        {
            get { return "<None>"; }
        }
    }

    public class HeatingLimit
    {
        public string Type
        {
            get { return "Autosize"; }
        }
    }

    public class CoolingLimit
    {
        public string Type
        {
            get { return "Autosize"; }
        }
    }
}
