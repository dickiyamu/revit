using System.Collections.Generic;
using Honeybee.Core;
// ReSharper disable UnusedMember.Global

namespace Honeybee.Revit.Schemas.Enumerations
{
    public class BuildingPrograms : Enumeration
    {
        public static readonly BuildingPrograms LargeOffice = new BuildingPrograms(0, "LargeOffice");
        public static readonly BuildingPrograms MediumOffice = new BuildingPrograms(1, "MediumOffice");
        public static readonly BuildingPrograms SmallOffice = new BuildingPrograms(2, "SmallOffice");
        public static readonly BuildingPrograms MidriseApartment = new BuildingPrograms(3, "MidriseApartment");
        public static readonly BuildingPrograms HighriseApartment = new BuildingPrograms(4, "HighriseApartment");
        public static readonly BuildingPrograms Retail = new BuildingPrograms(5, "Retail");
        public static readonly BuildingPrograms StripMall = new BuildingPrograms(6, "StripMall");
        public static readonly BuildingPrograms PrimarySchool = new BuildingPrograms(7, "PrimarySchool");
        public static readonly BuildingPrograms SecondarySchool = new BuildingPrograms(8, "SecondarySchool");
        public static readonly BuildingPrograms SmallHotel = new BuildingPrograms(9, "SmallHotel");
        public static readonly BuildingPrograms LargeHotel = new BuildingPrograms(10, "LargeHotel");
        public static readonly BuildingPrograms Hospital = new BuildingPrograms(11, "Hospital");
        public static readonly BuildingPrograms Outpatient = new BuildingPrograms(12, "Outpatient");
        public static readonly BuildingPrograms Laboratory = new BuildingPrograms(13, "Laboratory");
        public static readonly BuildingPrograms Warehouse = new BuildingPrograms(14, "Warehouse");
        public static readonly BuildingPrograms SuperMarket = new BuildingPrograms(15, "SuperMarket");
        public static readonly BuildingPrograms FullServiceRestaurant = new BuildingPrograms(16, "FullServiceRestaurant");
        public static readonly BuildingPrograms QuickServiceRestaurant = new BuildingPrograms(17, "QuickServiceRestaurant");
        public static readonly BuildingPrograms Courthouse = new BuildingPrograms(18, "Courthouse");
        public static readonly BuildingPrograms LargeDataCenterHighIte = new BuildingPrograms(19, "LargeDataCenterHighITE");
        public static readonly BuildingPrograms LargeDataCenterLowIte = new BuildingPrograms(20, "LargeDataCenterLowITE");
        public static readonly BuildingPrograms SmallDataCenterHighIte = new BuildingPrograms(21, "SmallDataCenterHighITE");
        public static readonly BuildingPrograms SmallDataCenterLowIte = new BuildingPrograms(22, "SmallDataCenterLowITE");

        public BuildingPrograms()
        {
        }

        private BuildingPrograms(int value, string displayName) : base(value, displayName)
        {
        }

        public IEnumerable<BuildingPrograms> GetAll()
        {
            return GetAll<BuildingPrograms>();
        }
    }
}
