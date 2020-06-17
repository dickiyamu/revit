#region References

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Honeybee.Core;
using Honeybee.Revit.CreateModel;
using Honeybee.Revit.ModelSettings;
using Honeybee.Revit.SharedProject.Utilities;
using NLog;

#endregion

namespace Honeybee.Revit
{
    public class AppCommand : IExternalApplication
    {
        private static Logger _logger;

        public static CreateModelRequestHandler CreateModelHandler { get; set; }
        public static ExternalEvent CreateModelEvent { get; set; }
        public static AnnotationUpdater AnnotationUpdater { get; set; }

        internal Document ActiveDoc { get; set; }

        public Result OnStartup(UIControlledApplication app)
        {
            // (Konrad) Initiate Nlog logger.
            NLogUtils.CreateConfiguration();
            _logger = LogManager.GetCurrentClassLogger();

            // (Konrad) Setup Document events.
            app.ControlledApplication.DocumentOpened += OnDocumentOpened;
            app.ControlledApplication.DocumentCreated += OnDocumentCreated;
            app.ControlledApplication.DocumentSaving += OnDocumentSaving;
            app.ControlledApplication.DocumentSynchronizingWithCentral += OnDocumentSynchronizingWithCentral;
            app.ControlledApplication.DocumentSynchronizedWithCentral += OnDocumentSynchronizedWithCentral;
            app.ControlledApplication.FailuresProcessing += FailureProcessor.CheckFailure;
            app.ViewActivated += OnViewActivated;

            app.CreateRibbonTab("Honeybee");
            var panel = app.CreateRibbonPanel("Honeybee", "Honeybee");

            SettingsCommand.CreateButton(panel);
            var hbButton = CreateHbModelCommand.CreateButton();
            var dfButton = CreateDfModelCommand.CreateButton();
            var splitButton = new SplitButtonData("CreateModelCommand", "Split");
            var sb = (SplitButton)panel.AddItem(splitButton);
            sb.AddPushButton(hbButton);
            sb.AddPushButton(dfButton);

            CreateModelHandler = new CreateModelRequestHandler();
            CreateModelEvent = ExternalEvent.Create(CreateModelHandler);

            // (Konrad) Register an updater that will watch annotations for changes.
            AnnotationUpdater = new AnnotationUpdater(app.ActiveAddInId);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            app.ControlledApplication.DocumentOpened -= OnDocumentOpened;
            app.ControlledApplication.DocumentCreated -= OnDocumentCreated;
            app.ControlledApplication.DocumentSaving -= OnDocumentSaving;
            app.ControlledApplication.DocumentSynchronizingWithCentral -= OnDocumentSynchronizingWithCentral;
            app.ControlledApplication.DocumentSynchronizedWithCentral -= OnDocumentSynchronizedWithCentral;
            app.ControlledApplication.FailuresProcessing -= FailureProcessor.CheckFailure;
            app.ViewActivated -= OnViewActivated;

            return Result.Succeeded;
        }

        #region Event Handlers

        private static void OnDocumentSynchronizedWithCentral(object sender, DocumentSynchronizedWithCentralEventArgs e)
        {
            FailureProcessor.IsSynchronizing = false;
        }

        private static void OnDocumentSynchronizingWithCentral(object sender, DocumentSynchronizingWithCentralEventArgs e)
        {
            FailureProcessor.IsSynchronizing = true;

            var doc = e.Document;
            if (doc == null) return;

            try
            {
                SchemaUtils.SaveSchema(doc);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
            }
        }

        private void OnViewActivated(object sender, ViewActivatedEventArgs e)
        {
            var doc = e.CurrentActiveView.Document;
            if (doc == null || doc.IsFamilyDocument || !DocumentChanged(doc)) return;

            var uiApp = (UIApplication)sender;
            if (uiApp != null)
            {
                CheckIn(doc);
            }
        }

        private void OnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            try
            {
                var doc = e.Document;
                if (doc == null || e.IsCancelled() || doc.IsFamilyDocument) return;

                CheckIn(doc);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
            }
        }

        private void OnDocumentCreated(object sender, DocumentCreatedEventArgs e)
        {
            try
            {
                var doc = e.Document;
                if (doc == null || e.IsCancelled() || doc.IsFamilyDocument) return;

                CheckIn(doc);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
            }
        }

        private static void OnDocumentSaving(object sender, DocumentSavingEventArgs e)
        {
            var doc = e.Document;
            if (doc == null) return;

            try
            {
                SchemaUtils.SaveSchema(doc);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
            }
        }

        #endregion

        #region Utilities

        private void CheckIn(Document doc)
        {
            ActiveDoc = doc;

            var settings = SchemaUtils.LoadSchema(doc);
            AppSettings.Instance.StoredSettings = settings ?? new StoredSettings();
        }

        private bool DocumentChanged(Document doc)
        {
            return ActiveDoc.GetHashCode() != doc.GetHashCode();
        }

        #endregion
    }
}
