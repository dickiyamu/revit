using Autodesk.Revit.DB;
using Newtonsoft.Json;

namespace Honeybee.Revit.ModelSettings.Geometry.Wrappers
{
    public class GlazingTypeWrapper
    {
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string GlazingType { get; set; }

        [JsonConstructor]
        public GlazingTypeWrapper()
        {
        }

        public GlazingTypeWrapper(ElementType pt, string glazingType)
        {
            UniqueId = pt.UniqueId;
            Name = $"{pt.FamilyName} - {pt.Name}";
        }

        public override bool Equals(object obj)
        {
            return obj is GlazingTypeWrapper item && UniqueId.Equals(item.UniqueId);
        }

        public override int GetHashCode()
        {
            return UniqueId.GetHashCode();
        }
    }
}
