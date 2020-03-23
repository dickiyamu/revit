#region Properties

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DragonflySchema;
using Honeybee.Core;
using Honeybee.Revit.CreateModel.Wrappers;
using Honeybee.Revit.Schemas;
using Honeybee.Revit.Schemas.Converters;
using Newtonsoft.Json;
using NLog;
using DF = DragonflySchema;
using Room2D = Honeybee.Revit.Schemas.Room2D;
using Surface = Honeybee.Revit.Schemas.Surface;

#endregion

namespace Honeybee.Revit.CreateModel
{
    public class CreateModelModel
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private Document Doc { get; }
        private UIDocument UiDoc { get; }

        public CreateModelModel(UIDocument uiDoc)
        {
            Doc = uiDoc.Document;
            UiDoc = uiDoc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<SpatialObjectWrapper> GetSpatialObjects()
        {
            var objects = new FilteredElementCollector(Doc)
                .OfClass(typeof(SpatialElement))
                .WhereElementIsNotElementType()
                .Cast<SpatialElement>()
                .Where(x => (x is Room || x is Space) && x.Area > 0)
                .Select(x => new SpatialObjectWrapper(x))
                .OrderBy(x => x.Level.Elevation)
                .ToList();

            AssignBoundaryConditions(objects);

            return objects;
        }

        public void WriteJournalComment(string comment)
        {
            AppCommand.CreateModelHandler.Arg1 = comment;
            AppCommand.CreateModelHandler.Request.Make(RequestId.WriteJournalComment);
            AppCommand.CreateModelEvent.Raise();
        }

        public void ShowBoundaryConditions(SpatialObjectWrapper so)
        {
            AppCommand.CreateModelHandler.Arg1 = so;
            AppCommand.CreateModelHandler.Request.Make(RequestId.ShowBoundaryConditions);
            AppCommand.CreateModelEvent.Raise();
        }

        public void SerializeRoom2D(List<Room2D> rooms)
        {
            try
            {
                var dfObjects = rooms.Select(x => x.ToDragonfly());
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new BoundaryConditionsConverter());
                var json = JsonConvert.SerializeObject(dfObjects, Formatting.Indented, settings);
                if (string.IsNullOrWhiteSpace(json)) return;

                // TODO: This should produce a dialog for users to save the JSON. For now.
                const string filePath = @"C:\Users\ksobon\Desktop\Honebee.json";
                var dir = Path.GetDirectoryName(filePath);
                if (string.IsNullOrWhiteSpace(dir)) return;

                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                if (File.Exists(filePath)) FileUtils.TryDeleteFile(filePath);

                File.WriteAllText(filePath, json);
            }
            catch(Exception e)
            {
                _logger.Fatal(e);
            }
        }

        public List<SpatialObjectWrapper> SelectRoomsSpaces()
        {
            var result = new List<SpatialObjectWrapper>();
            try
            {
                var selection = UiDoc.Selection.PickObjects(ObjectType.Element, new FilterRoomsSpaces(), "Please select Rooms/Spaces.");
                return selection.Select(x => new SpatialObjectWrapper(Doc.GetElement(x.ElementId))).ToList();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                _logger.Info("Selection was cancelled.");
                return result;
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                return result;
            }
        }

        #region Utilities

        private static void AssignBoundaryConditions(IReadOnlyCollection<SpatialObjectWrapper> objects)
        {
            foreach (var so in objects)
            {
                var bcs = new List<BoundaryCondition>();
                foreach (var curve in so.Room2D.FloorBoundarySegments)
                {
                    bcs.Add(new Surface().Init(objects.Where(x => !Equals(x, so)), curve));
                }

                so.Room2D.BoundaryConditions = bcs;
            }
        }

        #endregion
    }

    public class FilterRoomsSpaces : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem.Category.Id.IntegerValue == BuiltInCategory.OST_Rooms.GetHashCode() ||
                   elem.Category.Id.IntegerValue == BuiltInCategory.OST_MEPSpaces.GetHashCode();
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
