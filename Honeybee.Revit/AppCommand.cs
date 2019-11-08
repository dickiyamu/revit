using Autodesk.Revit.UI;
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

        public Result OnStartup(UIControlledApplication app)
        {
            // (Konrad) Initiate Nlog logger.
            NLogUtils.CreateConfiguration();
            _logger = LogManager.GetCurrentClassLogger();

            app.CreateRibbonTab("Honeybee");
            var panel = app.CreateRibbonPanel("Honeybee", "Honeybee");

            CreateModelCommand.CreateButton(panel);

            CreateModelHandler = new CreateModelRequestHandler();
            CreateModelEvent = ExternalEvent.Create(CreateModelHandler);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }
    }
}
