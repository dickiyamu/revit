using System.Collections.Generic;
using Honeybee.Core;
// ReSharper disable UnusedMember.Global

namespace Honeybee.Revit.Schemas.Enumerations
{
    public class ConstructionTypes : Enumeration
    {
        public static readonly ConstructionTypes SteelFramed = new ConstructionTypes(0, "SteelFramed");
        public static readonly ConstructionTypes WoodFramed = new ConstructionTypes(1, "WoodFramed");
        public static readonly ConstructionTypes Mass = new ConstructionTypes(2, "Mass");
        public static readonly ConstructionTypes MetalBuilding = new ConstructionTypes(2, "Metal Building");

        public ConstructionTypes()
        {
        }

        private ConstructionTypes(int value, string displayName) : base(value, displayName)
        {
        }

        public IEnumerable<ConstructionTypes> GetAll()
        {
            return GetAll<ConstructionTypes>();
        }
    }
}
