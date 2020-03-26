using System.Collections.Generic;
using System.Windows.Annotations;
using System.Windows.Documents;
using Autodesk.Revit.DB;
using Honeybee.Revit.CreateModel.Wrappers;

namespace Honeybee.Revit.CreateModel
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

    public class SurfaceAdjacentRoomChanged
    {
        public AnnotationWrapper Annotation { get; set; }

        public SurfaceAdjacentRoomChanged(AnnotationWrapper aw)
        {
            Annotation = aw;
        }
    }

    public class AnnotationsCreated
    {
        public SpatialObjectWrapper SpatialObject { get; set; }

        public AnnotationsCreated(SpatialObjectWrapper sow)
        {
            SpatialObject = sow;
        }
    }
}
