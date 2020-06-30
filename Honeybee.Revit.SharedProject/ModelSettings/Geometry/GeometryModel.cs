using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Honeybee.Revit.ModelSettings.Geometry.Wrappers;
using NLog;

namespace Honeybee.Revit.ModelSettings.Geometry
{
    public class GeometryModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private Document Doc { get; }
        private UIDocument UiDoc { get; }

        public GeometryModel(UIDocument uiDoc)
        {
            Doc = uiDoc.Document;
            UiDoc = uiDoc;
        }

        public List<PanelTypeWrapper> CollectPanels()
        {
            var result = new List<PanelTypeWrapper>();
            var panels = new FilteredElementCollector(Doc)
                .OfClass(typeof(PanelType))
                .WhereElementIsElementType()
                .Cast<PanelType>()
                .ToList();

            if (panels.Any())
            {
                var cwDoors = new FilteredElementCollector(Doc, panels.First().GetSimilarTypes())
                    .OfCategory(BuiltInCategory.OST_Doors)
                    .Cast<ElementType>()
                    .Select(x => new PanelTypeWrapper(x));

                result.AddRange(cwDoors);
            }

            result.AddRange(panels.Select(x => new PanelTypeWrapper(x)));

            return result.Except(AppSettings.Instance.StoredSettings.GeometrySettings.GlazingTypes).ToList();
        }
    }
}
