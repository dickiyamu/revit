using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NLog;

namespace Honeybee.Revit.ModelSettings
{
    public class SettingsModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private Document Doc { get; }
        public UIDocument UiDoc { get; }

        public SettingsModel(UIDocument uiDoc)
        {
            Doc = uiDoc.Document;
            UiDoc = uiDoc;
        }
    }
}
