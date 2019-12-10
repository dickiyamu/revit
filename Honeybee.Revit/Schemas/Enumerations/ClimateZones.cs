using System.Collections.Generic;
using Honeybee.Core;
// ReSharper disable UnusedMember.Global

namespace Honeybee.Revit.Schemas.Enumerations
{
    public class ClimateZones : Enumeration
    {
        public static readonly ClimateZones VeryHot = new ClimateZones(0, "ClimateZone1");
        public static readonly ClimateZones Hot = new ClimateZones(1, "ClimateZone2");
        public static readonly ClimateZones Warm = new ClimateZones(2, "ClimateZone3");
        public static readonly ClimateZones Mixed = new ClimateZones(3, "ClimateZone4");
        public static readonly ClimateZones Cool = new ClimateZones(4, "ClimateZone5");
        public static readonly ClimateZones Cold = new ClimateZones(5, "ClimateZone6");
        public static readonly ClimateZones VeryCold = new ClimateZones(6, "ClimateZone7");
        public static readonly ClimateZones Subarctic = new ClimateZones(7, "ClimateZone8");

        public ClimateZones()
        {
        }

        private ClimateZones(int value, string displayName) : base(value, displayName)
        {
        }

        public IEnumerable<ClimateZones> GetAll()
        {
            return GetAll<ClimateZones>();
        }
    }
}
