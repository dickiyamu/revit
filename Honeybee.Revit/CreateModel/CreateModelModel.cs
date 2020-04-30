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
using Honeybee.Core;
using Honeybee.Revit.CreateModel.Wrappers;
using Honeybee.Revit.Schemas;
using Newtonsoft.Json;
using NLog;
using DF = DragonflySchema;
using HB = HoneybeeSchema;
using Surface = Honeybee.Revit.Schemas.Surface;
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

            //var stories = objects
            //    .Select(x => x.Room2D)
            //    .GroupBy(x => x.Level.Name)
            //    .Select(x => new Story(x.Key, x.ToList(), new StoryPropertiesAbridged()))
            //    .ToList();

            //DF.Model.UnitsEnum dfUnits;
            //var units = Doc.GetUnits();
            //var unitType = units.GetFormatOptions(UnitType.UT_Length).DisplayUnits;
            //switch (unitType)
            //{
            //    case DisplayUnitType.DUT_METERS:
            //        dfUnits = DF.Model.UnitsEnum.Meters;
            //        break;
            //    case DisplayUnitType.DUT_CENTIMETERS:
            //        dfUnits = DF.Model.UnitsEnum.Centimeters;
            //        break;
            //    case DisplayUnitType.DUT_MILLIMETERS:
            //        dfUnits = DF.Model.UnitsEnum.Millimeters;
            //        break;
            //    case DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES:
            //        dfUnits = DF.Model.UnitsEnum.Millimeters;
            //        break;
            //    case DisplayUnitType.DUT_MILLIMETERS:
            //        dfUnits = DF.Model.UnitsEnum.Millimeters;
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}

            //var building = new Building("Building 1", stories, new BuildingPropertiesAbridged());
            //var model = new Model("Model 1", new List<Building> {building}, new ModelProperties())
            //{
            //    Units = DF.Model.UnitsEnum.Feet,
            //    Tolerance = 0.0001d
            //};
            //var dfModel = model.ToDragonfly();
            //var json = JsonConvert.SerializeObject(dfModel, Formatting.Indented, new DF.AnyOfJsonConverter());
            //var jsonPath = Path.Combine(Path.GetTempPath(), "Dragonfly.json"); 
            //if (File.Exists(jsonPath)) FileUtils.TryDeleteFile(jsonPath);
            //File.WriteAllText(jsonPath, json);

            //var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            //if (path == null) return objects;

            //string pyPath = null;
            //foreach (var p in path.Split(';'))
            //{
            //    var fullPath = Path.Combine(p, "python.exe");
            //    if (!File.Exists(fullPath)) continue;

            //    pyPath = fullPath;
            //    break;
            //}

            //if (string.IsNullOrWhiteSpace(pyPath)) return objects;

            //var pyDir = Path.GetDirectoryName(pyPath);
            //if (pyDir == null) return objects;

            //var dfPath = Path.Combine(pyDir, "Scripts", "dragonfly.exe");
            //if (!File.Exists(dfPath)) return objects;

            //var jsonResult = RunCommand(pyPath, dfPath, jsonPath);
            //JsonConverter[] converters =
            //{
            //    new DF.AnyOfJsonConverter(),
            //    new ConstructionBaseConverter(),
            //    new MaterialBaseConverter(),
            //    new Point2DConverter(),
            //    new BoundaryConditionBaseConverter(),
            //    new WindowParameterBaseConverter()
            //};
            //var resultModel = JsonConvert.DeserializeObject<Model>(jsonResult, converters);

            //foreach (var b in resultModel.Buildings)
            //{
            //    foreach (var s in b.UniqueStories)
            //    {
            //        foreach (var r in s.Room2Ds)
            //        {
            //            var room = objects.FirstOrDefault(x => Equals(x.Room2D, r));
            //            if (room == null) continue;

            //            room.Room2D.BoundaryConditions = r.BoundaryConditions ?? new List<BoundaryConditionBase>();
            //            room.Room2D.FloorBoundary = r.FloorBoundary ?? new List<Point2D>();
            //            room.Room2D.FloorHoles = r.FloorHoles ?? new List<List<Point2D>>();
            //        }
            //    }
            //}

            AssignBoundaryConditions(objects);

            return objects;
        }

        //public string RunCommand(string pyPath, string dfPath, string jsonPath)
        //{
        //    var ps = PowerShell.Create();
        //    ps.AddCommand(pyPath)
        //        .AddParameter("-m")
        //        .AddCommand(dfPath)
        //        .AddArgument("edit")
        //        .AddArgument("solve-adjacency")
        //        .AddArgument(jsonPath);
        //    var psObject = ps.Invoke();
        //    var result = psObject.FirstOrDefault()?.ImmediateBaseObject as string;
        //    ps.Commands.Clear();

        //    return result;
        //}

        public void ShowBoundaryConditions(SpatialObjectWrapper so)
        {
            AppCommand.CreateModelHandler.Arg1 = so;
            AppCommand.CreateModelHandler.Request.Make(RequestId.ShowBoundaryConditions);
            AppCommand.CreateModelEvent.Raise();
        }

        public void SerializeRoom2D(List<Room2D> rooms, bool dragonfly = true)
        {
            try
            {
                if (dragonfly)
                {
                    var stories = rooms
                        .GroupBy(x => x.Level.Name)
                        .Select(x => new Story(x.Key, x.ToList(), new StoryPropertiesAbridged()))
                        .ToList();

                    var building = new Building("Building 1", stories, new BuildingPropertiesAbridged());
                    var model = new Model("Model 1", new List<Building> { building }, new ModelProperties())
                    {
                        Units = DF.Model.UnitsEnum.Feet,
                        Tolerance = 0.0001d
                    };
                    var dfModel = model.ToDragonfly();
                    var json = JsonConvert.SerializeObject(dfModel, Formatting.Indented, new DF.AnyOfJsonConverter());
                    if (string.IsNullOrWhiteSpace(json)) return;

                    const string filePath = @"C:\Users\ksobon\Desktop\Dragonfly.json";
                    var dir = Path.GetDirectoryName(filePath);
                    if (string.IsNullOrWhiteSpace(dir)) return;

                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    if (File.Exists(filePath)) FileUtils.TryDeleteFile(filePath);

                    File.WriteAllText(filePath, json);
                }
                else
                {
                    var hbModel = rooms.Select(x => x.ToHoneybee());
                    var json = JsonConvert.SerializeObject(hbModel, Formatting.Indented, new HB.AnyOfJsonConverter());
                    if (string.IsNullOrWhiteSpace(json)) return;

                    const string filePath = @"C:\Users\ksobon\Desktop\Honeybee.json";
                    var dir = Path.GetDirectoryName(filePath);
                    if (string.IsNullOrWhiteSpace(dir)) return;

                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    if (File.Exists(filePath)) FileUtils.TryDeleteFile(filePath);

                    File.WriteAllText(filePath, json);
                }
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
                var offset = so.Self.get_Parameter(BuiltInParameter.ROOM_LOWER_OFFSET).AsDouble();
                var boundaryCurves = so.Room2D.FloorBoundary.GetCurves(so.Level.Elevation + offset);
                for (var i = 0; i < boundaryCurves.Count; i++)
                {
                    var currentBc = so.Room2D.BoundaryConditions[i];
                    if (currentBc is Surface) continue;

                    // (Konrad) Adiabatic can only be assigned to Boundaries that don't have a Window.
                    var allowAdiabatic = so.Room2D.WindowParameters[i] == null;
                    var bc = BoundaryConditionBase.Init(objects.Where(x => !Equals(x, so) && x.Level.Id == so.Level.Id), boundaryCurves[i], so, allowAdiabatic);
                    if (bc is Outdoors) continue;

                    so.Room2D.BoundaryConditions[i] = bc;
                }

                var holeCurves = so.Room2D.FloorHoles.SelectMany(x => x.GetCurves(so.Level.Elevation + offset)).ToList();
                for (var i = 0; i < holeCurves.Count; i++)
                {
                    var currentBc = so.Room2D.BoundaryConditions[boundaryCurves.Count + i];
                    if (currentBc is Surface) continue;

                    // (Konrad) Adiabatic can only be assigned to Boundaries that don't have a Window.
                    var allowAdiabatic = so.Room2D.WindowParameters[boundaryCurves.Count + i] == null;
                    var bc = BoundaryConditionBase.Init(objects.Where(x => !Equals(x, so) && x.Level.Id == so.Level.Id), holeCurves[i], so, allowAdiabatic);
                    if (bc is Outdoors) continue;

                    so.Room2D.BoundaryConditions[boundaryCurves.Count + i] = bc;
                }
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
