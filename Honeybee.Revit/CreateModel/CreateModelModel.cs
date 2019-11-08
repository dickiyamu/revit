using Autodesk.Revit.DB;
using NLog;

namespace Honeybee.Revit.CreateModel
{
    public class CreateModelModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private Document Doc { get; }

        public CreateModelModel(Document doc)
        {
            Doc = doc;
        }
    }
}
