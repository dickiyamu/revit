﻿#region References

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Honeybee.Core;
using NLog;

#endregion

namespace Honeybee.Revit.CreateModel
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CreateHbModelCommand : IExternalCommand
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static CreateModelView View { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                _logger.Info("Create Model started.");

                var uiApp = commandData.Application;
                var uiDoc = uiApp.ActiveUIDocument;

                if (View != null)
                {
                    if (View.WindowState == WindowState.Minimized) View.WindowState = WindowState.Normal;
                    View.Activate();

                    return Result.Succeeded;
                }

                var m = new CreateModelModel(uiDoc);
                var vm = new CreateModelViewModel(m, false);
                var v = new CreateModelView
                {
                    DataContext = vm
                };

                View = v;
                View.Closing += OnViewClosing;

                var unused = new WindowInteropHelper(v)
                {
                    Owner = Process.GetCurrentProcess().MainWindowHandle
                };

                v.Show();

                _logger.Info("Create Model ended.");

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                _logger.Fatal(e);

                return Result.Failed;
            }
        }

        public static PushButtonData CreateButton()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var data = new PushButtonData(
                "CreateHbModelCommand",
                "Honeybee Model",
                assembly.Location,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName)
            {
                ToolTip = "Description...",
                LargeImage = ImageUtils.LoadImage(assembly, "_32x32.createModel.png")
            };

            return data;
        }

        private static void OnViewClosing(object sender, CancelEventArgs e)
        {
            View = null;
            if (sender is CreateModelView view) view.Closing -= OnViewClosing;

            _logger.Info("Create Model ended.");
        }
    }
}
