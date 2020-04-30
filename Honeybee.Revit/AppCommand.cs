//using System;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.Events;

using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
//using Autodesk.Revit.UI.Events;
using Honeybee.Core;
using Honeybee.Revit.CreateModel;
using NLog;

namespace Honeybee.Revit
{
    public class AppCommand : IExternalApplication
    {
        private static Logger _logger;

        public static CreateModelRequestHandler CreateModelHandler { get; set; }
        public static ExternalEvent CreateModelEvent { get; set; }
        public static AnnotationUpdater AnnotationUpdater { get; set; }

        //internal Document ActiveDoc { get; set; }

        public Result OnStartup(UIControlledApplication app)
        {
            // (Konrad) Initiate Nlog logger.
            NLogUtils.CreateConfiguration();
            _logger = LogManager.GetCurrentClassLogger();

            //// (Konrad) Setup Document events.
            //app.ControlledApplication.DocumentOpened += OnDocumentOpened;
            //app.ControlledApplication.DocumentCreated += OnDocumentCreated;
            //app.ViewActivated += OnViewActivated;
            app.ControlledApplication.FailuresProcessing += FailureProcessor.CheckFailure;
            app.ControlledApplication.DocumentSynchronizingWithCentral += OnDocumentSynchronizingWithCentral;
            app.ControlledApplication.DocumentSynchronizedWithCentral += OnDocumentSynchronizedWithCentral;

            app.CreateRibbonTab("Honeybee");
            var panel = app.CreateRibbonPanel("Honeybee", "Honeybee");

            CreateModelCommand.CreateButton(panel);

            CreateModelHandler = new CreateModelRequestHandler();
            CreateModelEvent = ExternalEvent.Create(CreateModelHandler);

            // (Konrad) Register an updater that will watch Project Information for changes.
            AnnotationUpdater = new AnnotationUpdater(app.ActiveAddInId);

            return Result.Succeeded;
        }

        private static void OnDocumentSynchronizedWithCentral(object sender, DocumentSynchronizedWithCentralEventArgs e)
        {
            FailureProcessor.IsSynchronizing = false;
        }

        private static void OnDocumentSynchronizingWithCentral(object sender, DocumentSynchronizingWithCentralEventArgs e)
        {
            FailureProcessor.IsSynchronizing = true;
        }

        //private void OnViewActivated(object sender, ViewActivatedEventArgs e)
        //{
        //    var doc = e.CurrentActiveView.Document;
        //    if (doc == null || doc.IsFamilyDocument || !DocumentChanged(doc)) return;

        //    var uiApp = (UIApplication)sender;
        //    if (uiApp != null)
        //    {
        //        CheckIn(doc);
        //    }
        //}

        //private void OnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        //{
        //    try
        //    {
        //        var doc = e.Document;
        //        if (doc == null || e.IsCancelled() || doc.IsFamilyDocument) return;

        //        CheckIn(doc);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Fatal(ex);
        //    }
        //}

        //private void OnDocumentCreated(object sender, DocumentCreatedEventArgs e)
        //{
        //    try
        //    {
        //        var doc = e.Document;
        //        if (doc == null || e.IsCancelled() || doc.IsFamilyDocument) return;

        //        CheckIn(doc);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Fatal(ex);
        //    }
        //}

        public Result OnShutdown(UIControlledApplication app)
        {
            //app.ControlledApplication.DocumentOpened -= OnDocumentOpened;
            //app.ControlledApplication.DocumentCreated -= OnDocumentCreated;
            //app.ViewActivated -= OnViewActivated;
            app.ControlledApplication.FailuresProcessing -= FailureProcessor.CheckFailure;
            app.ControlledApplication.DocumentSynchronizingWithCentral -= OnDocumentSynchronizingWithCentral;
            app.ControlledApplication.DocumentSynchronizedWithCentral -= OnDocumentSynchronizedWithCentral;

            return Result.Succeeded;
        }

        //private void CheckIn(Document doc)
        //{
        //    AnnotationUpdater.Register(doc);
        //}

        //private bool DocumentChanged(Document doc)
        //{
        //    return ActiveDoc.GetHashCode() != doc.GetHashCode();
        //}
    }
}
