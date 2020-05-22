using System;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Honeybee.Core;
using NLog;

namespace Honeybee.Revit.ModelSettings
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class SettingsCommand : IExternalCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                _logger.Info("Settings started.");

                var uiApp = commandData.Application;
                var uiDoc = uiApp.ActiveUIDocument;

                var vm = new SettingsViewModel(0);
                var v = new SettingsView
                {
                    DataContext = vm
                };

                v.Show();

                _logger.Info("Settings ended.");

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                _logger.Fatal(e);

                return Result.Failed;
            }
        }

        public static void CreateButton(RibbonPanel panel)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var unused = (PushButton)panel.AddItem(
                new PushButtonData(
                    "SettingsCommand",
                    "Settings",
                    assembly.Location,
                    MethodBase.GetCurrentMethod().DeclaringType?.FullName)
                {
                    ToolTip = "Description...",
                    LargeImage = ImageUtils.LoadImage(assembly, typeof(AppCommand).Namespace, "_32x32.settings.png")
                });
        }
    }
}
