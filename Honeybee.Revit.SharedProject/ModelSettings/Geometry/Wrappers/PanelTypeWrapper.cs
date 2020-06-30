using Autodesk.Revit.DB;
using Newtonsoft.Json;

namespace Honeybee.Revit.ModelSettings.Geometry.Wrappers
{
    public class PanelTypeWrapper
    {
        public string UniqueId { get; set; }
        public string Name { get; set; }

        [JsonConstructor]
        public PanelTypeWrapper()
        {
        }

        public PanelTypeWrapper(ElementType pt)
        {
            UniqueId = pt.UniqueId;
            Name = $"{pt.FamilyName} - {pt.Name}";
        }

        public override bool Equals(object obj)
        {
            return obj is PanelTypeWrapper item && UniqueId.Equals(item.UniqueId);
        }

        public override int GetHashCode()
        {
            return UniqueId.GetHashCode();
        }
    }
}
