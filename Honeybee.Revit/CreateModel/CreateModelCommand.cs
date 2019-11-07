using System;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Honeybee.Core;
using NLog;

namespace Honeybee.Revit.CreateModel
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CreateModelCommand : IExternalCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                _logger.Info("Create Model started.");

                //TODO: Reference UI.

                _logger.Info("Create Model ended.");

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                _logger.Fatal(e);

                return Result.Failed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panel"></param>
        public static void CreateButton(RibbonPanel panel)
        {
            //TODO: Create button icon.
            //var assembly = Assembly.GetExecutingAssembly();
            //var unused = (PushButton)panel.AddItem(
            //    new PushButtonData(
            //        "CreateModelCommand",
            //        "Create Model",
            //        assembly.Location,
            //        MethodBase.GetCurrentMethod().DeclaringType?.FullName)
            //    {
            //        ToolTip = "Description...",
            //        LargeImage = ImageUtils.LoadImage(assembly, typeof(AppCommand).Namespace, "_32x32.createModel.png")
            //    });
        }
    }
}
