#region Properties

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Honeybee.Core;
using Honeybee.Revit.CreateModel.Wrappers;
using Honeybee.Revit.Schemas;
using Honeybee.Revit.Schemas.Converters;
using Newtonsoft.Json;
using NLog;
using DF = DragonflySchema;
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

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

            //return objects;

            var stories = objects
                .Select(x => x.Room2D)
                .GroupBy(x => x.Level.Name)
                .Select(x => new Story(x.Key, x.ToList(), new StoryPropertiesAbridged()))
                .ToList();

            var building = new Building("Building 1", stories, new BuildingPropertiesAbridged());

            var model = new Model("Model 1", new List<Building> { building }, new ModelProperties());
            var dfModel = model.ToDragonfly();
            var json = JsonConvert.SerializeObject(dfModel, Formatting.Indented, new DF.AnyOfJsonConverter());
            var jsonPath = Path.Combine(Path.GetTempPath(), "Dragonfly.json"); 
            if (File.Exists(jsonPath)) FileUtils.TryDeleteFile(jsonPath);
            File.WriteAllText(jsonPath, json);

            var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            if (path == null) return objects;

            string pyPath = null;
            foreach (var p in path.Split(';'))
            {
                var fullPath = Path.Combine(p, "python.exe");
                if (!File.Exists(fullPath)) continue;

                pyPath = fullPath;
                break;
            }

            if (string.IsNullOrWhiteSpace(pyPath)) return objects;

            var pyDir = Path.GetDirectoryName(pyPath);
            if (pyDir == null) return objects;

            var dfPath = Path.Combine(pyDir, "Scripts", "dragonfly.exe");
            if (!File.Exists(dfPath)) return objects;

            var jsonResult = RunCommand(pyPath, dfPath, jsonPath);
            JsonConverter[] converters =
            {
                new DF.AnyOfJsonConverter(),
                new ConstructionBaseConverter(),
                new MaterialBaseConverter(),
                new Point2DConverter(),
                new BoundaryConditionBaseConverter()
            };
            var resultModel = JsonConvert.DeserializeObject<Model>(jsonResult, converters);

            var rooms = new List<Room2D>();
            foreach (var b in resultModel.Buildings)
            {
                foreach (var s in b.UniqueStories)
                {
                    foreach (var r in s.Room2Ds)
                    {
                        rooms.Add(r);
                    }
                }
            }
            foreach (var so in objects)
            {
                var dfRoom = rooms.First(x => Equals(x, so.Room2D));
                so.Room2D.BoundaryConditions = dfRoom.BoundaryConditions;
                so.Room2D.FloorBoundary = dfRoom.FloorBoundary;
            }

            return objects;
        }

        public string RunCommand(string pyPath, string dfPath, string jsonPath)
        {
            var ps = PowerShell.Create();
            ps.AddCommand(pyPath)
                .AddParameter("-m")
                .AddCommand(dfPath)
                .AddArgument("edit")
                .AddArgument("solve-adjacency")
                .AddArgument(jsonPath);
            var psObject = ps.Invoke();
            var result = psObject.FirstOrDefault()?.ImmediateBaseObject as string;
            ps.Commands.Clear();

            return result;
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
                var stories = rooms.GroupBy(x => x.Level.Name)
                    .Select(x => new Story(x.Key, x.ToList(), new StoryPropertiesAbridged())).ToList();
                var building = new Building("Building 1", stories, new BuildingPropertiesAbridged());
                var model = new Model("Sample Model", new List<Building> {building}, new ModelProperties());

                var dfModel = model.ToDragonfly();
                //var dfObjects = rooms.Select(x => x.ToDragonfly());
                //var settings = new JsonSerializerSettings();
                //settings.Converters.Add(new BoundaryConditionsConverter());
                var json = JsonConvert.SerializeObject(dfModel, Formatting.Indented, new DF.AnyOfJsonConverter());
                if (string.IsNullOrWhiteSpace(json)) return;

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
                var bcs = new List<BoundaryConditionBase>();
                foreach (var curve in so.Room2D.FloorBoundary.GetCurves())
                {
                    bcs.Add(BoundaryConditionBase.Init(objects.Where(x => !Equals(x, so)), curve, so));
                }

                foreach (var curve in so.Room2D.FloorHoles.SelectMany(x => x.GetCurves()))
                {
                    bcs.Add(BoundaryConditionBase.Init(objects.Where(x => !Equals(x, so)), curve, so));
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
