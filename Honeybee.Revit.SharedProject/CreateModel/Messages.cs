using Honeybee.Revit.CreateModel.Wrappers;

namespace Honeybee.Revit.CreateModel
{
    public class SurfaceAdjacentRoomChanged
    {
        public AnnotationWrapper Annotation { get; set; }

        public SurfaceAdjacentRoomChanged(AnnotationWrapper aw)
        {
            Annotation = aw;
        }
    }

    public class TypeChanged
    {
        public AnnotationWrapper Annotation { get; set; }

        public TypeChanged(AnnotationWrapper aw)
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

    public class UpdateStatusBarMessage
    {
        public string Message { get; set; }

        public UpdateStatusBarMessage(string message)
        {
            Message = message;
        }
    }
}
