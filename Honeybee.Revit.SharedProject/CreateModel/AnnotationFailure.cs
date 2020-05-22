using System;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

namespace Honeybee.Revit.CreateModel
{
    public static class AnnotationFailure
    {
        public static bool IsElementModified { get; set; }
        public static Document CurrentDoc { get; set; }

        public static void ProcessFailure(object sender, FailuresProcessingEventArgs args)
        {
            try
            {
                if (!IsElementModified || CurrentDoc == null) return;

                FailureProcessor.IsFailureProcessing = true;
                var fa = args.GetFailuresAccessor();

                var result =
                    MessageBox.Show(
                        "I am sorry, but you are not allowed to assign Surface Boundary Condition manually, nor to modify any existing properties.");
                if (result == MessageBoxResult.OK)
                {
                    args.SetProcessingResult(FailureProcessingResult.ProceedWithRollBack);
                    var option = fa.GetFailureHandlingOptions();
                    option.SetClearAfterRollback(true);
                    fa.SetFailureHandlingOptions(option);
                }

                IsElementModified = false;
                FailureProcessor.IsFailureProcessing = false;
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }
}
