using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace Honeybee.Revit.CreateModel
{
    public class FilterRoomsSpaces : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category.Id.IntegerValue == BuiltInCategory.OST_Rooms.GetHashCode() ||
                   elem.Category.Id.IntegerValue == BuiltInCategory.OST_MEPSpaces.GetHashCode();
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    public class FilterPlanting : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category.Id.IntegerValue == BuiltInCategory.OST_Planting.GetHashCode();
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    public class FilterPlanarFace : ISelectionFilter
    {
        private readonly Document _doc;

        public FilterPlanarFace(Document doc)
        {
            _doc = doc;
        }

        public bool AllowElement(Element elem)
        {
            return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            var element = _doc.GetElement(reference);
            var planarFace = element.GetGeometryObjectFromReference(reference) as PlanarFace;
            return planarFace != null;
        }
    }
}
