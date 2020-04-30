using Autodesk.Revit.DB.Events;

namespace Honeybee.Revit.CreateModel
{
    public static class FailureProcessor
    {
        public static bool IsFailureFound;
        public static bool IsSynchronizing = false;
        public static bool IsFailureProcessing = false;

        public static void CheckFailure(object sender, FailuresProcessingEventArgs args)
        {
            if (IsFailureProcessing) return;
            if (IsSynchronizing) return;
            if (!IsFailureFound) return;

            if (AnnotationFailure.IsElementModified)
            {
                AnnotationFailure.ProcessFailure(sender, args);
            }

            IsFailureFound = false;
        }
    }
}
