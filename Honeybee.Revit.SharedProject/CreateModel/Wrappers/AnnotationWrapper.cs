using Autodesk.Revit.DB;

namespace Honeybee.Revit.CreateModel.Wrappers
{
    public class AnnotationWrapper
    {
        public string UniqueId { get; }
        public string FamilySymbolId { get; set; }
        public string FamilySymbolName { get; set; }
        public string AdjacentRoom { get; set; }

        public AnnotationWrapper(FamilyInstance fi)
        {
            UniqueId = fi.UniqueId;
            var fs = fi.Document.GetElement(fi.GetTypeId()) as FamilySymbol;
            FamilySymbolId = fs?.UniqueId;
            FamilySymbolName = fs?.Name;
            AdjacentRoom = fi.LookupParameter("AdjacentRoom")?.AsString();
        }

        public override bool Equals(object obj)
        {
            return obj is AnnotationWrapper item && UniqueId.Equals(item.UniqueId);
        }

        public override int GetHashCode()
        {
            return UniqueId.GetHashCode();
        }
    }
}
