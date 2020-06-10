﻿#region Properties

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using GalaSoft.MvvmLight.Messaging;
using Honeybee.Core;
using Honeybee.Revit.CreateModel.Wrappers;
using Honeybee.Revit.Schemas;
using Honeybee.Revit.Schemas.Honeybee;
using Newtonsoft.Json;
using NLog;
using HB = HoneybeeSchema;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
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

        public List<SpatialObjectWrapper> GetSpatialObjects()
        {
            var objects = new FilteredElementCollector(Doc)
                .OfClass(typeof(SpatialElement))
                .WhereElementIsNotElementType()
                .Cast<SpatialElement>()
                //.Where(x => (x is Room || x is Space) && x.Area > 0 && x.Name.Contains("318"))
                .Where(x => (x is Room || x is Space) && x.Area > 0)
                .Select(x => new SpatialObjectWrapper(x))
                .OrderBy(x => x.Level.Elevation)
                .ToList();

            DF_AssignBoundaryConditions(objects);
            HB_AssignBoundaryConditions(objects);

            return objects;
        }

        public string RunHoneybeeEnergyCommand(string command, IEnumerable<string> ids)
        {
            var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            if (path == null)
                return string.Empty;

            string pyPath = null;
            foreach (var p in path.Split(';'))
            {
                var fullPath = Path.Combine(p, "python.exe");
                if (!File.Exists(fullPath))
                    continue;

                pyPath = fullPath;
                break;
            }

            if (string.IsNullOrWhiteSpace(pyPath))
                return string.Empty;

            var pyDir = Path.GetDirectoryName(pyPath);
            if (pyDir == null)
                return string.Empty;

            var dfPath = Path.Combine(pyDir, "Scripts", "honeybee-energy.exe");
            if (!File.Exists(dfPath))
                return string.Empty;

            var ps = PowerShell.Create();
            ps.AddCommand(pyPath)
                .AddParameter("-m")
                .AddCommand(dfPath)
                .AddArgument("lib")
                .AddArgument(command)
                .AddArgument(ids);

            var psObject = ps.Invoke();
            var result = psObject.FirstOrDefault()?.ImmediateBaseObject as string;
            ps.Commands.Clear();

            return result;
        }

        public string RunHoneybeeEnergyCommand2(string command, IEnumerable<string> args)
        {
            var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
            if (path == null)
                return string.Empty;

            string pyPath = null;
            foreach (var p in path.Split(';'))
            {
                var fullPath = Path.Combine(p, "python.exe");
                if (!File.Exists(fullPath))
                    continue;

                pyPath = fullPath;
                break;
            }

            if (string.IsNullOrWhiteSpace(pyPath))
                return string.Empty;

            var pyDir = Path.GetDirectoryName(pyPath);
            if (pyDir == null)
                return string.Empty;

            var dfPath = Path.Combine(pyDir, "Scripts", "honeybee-energy.exe");
            if (!File.Exists(dfPath))
                return string.Empty;

            var ps = PowerShell.Create();
            ps.AddCommand(pyPath)
                .AddParameter("-m")
                .AddCommand(dfPath)
                .AddArgument("lib")
                .AddArgument(command);

            foreach (var arg in args)
            {
                ps.AddArgument(arg);
            }

            var psObject = ps.Invoke();
            var result = psObject.FirstOrDefault()?.ImmediateBaseObject as string;
            ps.Commands.Clear();

            return result;
        }

        public void ShowBoundaryConditions(SpatialObjectWrapper so)
        {
            AppCommand.CreateModelHandler.Arg1 = so;
            AppCommand.CreateModelHandler.Request.Make(RequestId.ShowBoundaryConditions);
            AppCommand.CreateModelEvent.Raise();
        }

        public string SerializeRoom2D(
            List<Room2D> rooms, 
            ProgramType bProgramType, 
            ConstructionSet bConstructionSet, 
            bool dragonfly,
            List<Shade> shades)
        {
            try
            {
                var modelFilePath = string.Empty;

                var hbProgramTypes = GetProgramTypeSet(rooms, bProgramType);
                var hbConstructionSets = GetConstructionSets(rooms, bConstructionSet);
                var properties = new ModelProperties { Energy = new ModelEnergyProperties() };
                var globalConstructionSet = GetDefaultConstructionSet();

                properties.Energy.ConstructionSets.Add(globalConstructionSet);
                properties.Energy.GlobalConstructionSet = "Default Generic Construction Set";

                hbProgramTypes.ForEach(x => properties.Energy.ProgramTypes.Add(x));
                hbConstructionSets.ForEach(x => properties.Energy.ConstructionSets.Add(x));

                if (dragonfly)
                {
                    var stories = rooms
                        .GroupBy(x => x.LevelName)
                        .Select(x => new Story(x.Key, x.ToList(), new StoryPropertiesAbridged
                        {
                            Energy = new StoryEnergyPropertiesAbridged
                            {
                                ConstructionSet = bConstructionSet?.Identifier
                            }
                        }))
                        .ToList();

                    var building = new Building("Building 1", stories,
                        new BuildingPropertiesAbridged
                        {
                            Energy = new BuildingEnergyPropertiesAbridged
                            {
                                ConstructionSet = bConstructionSet?.Identifier
                            }
                        });

                    var model = new Model("Model 1", new List<Building> { building }, new ModelProperties())
                    {
                        Units = HB.Units.Feet,
                        Tolerance = 0.0001d
                    };
                    var dfModel = model.ToDragonfly();
                    dfModel.Properties = properties.ToDragonfly();

                    var json = JsonConvert.SerializeObject(dfModel, Formatting.Indented, new HB.AnyOfJsonConverter());
                    if (string.IsNullOrWhiteSpace(json))
                        return modelFilePath;

                    var simulationDir = AppSettings.Instance.StoredSettings.SimulationSettings.SimulationFolder;
                    var filePath = Path.Combine(simulationDir, "Dragonfly.json");
                    if (string.IsNullOrWhiteSpace(simulationDir))
                        return modelFilePath;

                    if (!Directory.Exists(simulationDir))
                        Directory.CreateDirectory(simulationDir);

                    if (File.Exists(filePath))
                        FileUtils.TryDeleteFile(filePath);

                    File.WriteAllText(filePath, json);
                    modelFilePath = filePath;

                    return modelFilePath;
                }
                else
                {
                    var contextShades = shades ?? GetContextShades();
                    var model = new Model("Model 1", new List<HB.Room>(), new ModelProperties(), contextShades)
                    {
                        Units = HB.Units.Feet,
                        Tolerance = 0.0001d
                    };
                    
                    var hbRooms = rooms.Select(x =>
                    {
                        var hb = x.ToHoneybee();
                        hb.Properties.Energy.ConstructionSet = x.Properties.Energy.ConstructionSet.Identifier;
                        hb.Properties.Energy.ProgramType = x.Properties.Energy.ProgramType.Identifier;
                        return hb;
                    }).ToList();

                    model.Rooms = hbRooms;

                    var hbModel = model.ToHoneybee();
                    hbModel.Properties = properties.ToHoneybee();

                    var json = JsonConvert.SerializeObject(hbModel, Formatting.Indented, new HB.AnyOfJsonConverter());
                    if (string.IsNullOrWhiteSpace(json))
                        return modelFilePath;

                    var simulationDir = AppSettings.Instance.StoredSettings.SimulationSettings.SimulationFolder;
                    var filePath = Path.Combine(simulationDir, "Honeybee.json");
                    if (string.IsNullOrWhiteSpace(simulationDir))
                        return modelFilePath;

                    if (!Directory.Exists(simulationDir))
                        Directory.CreateDirectory(simulationDir);

                    if (File.Exists(filePath))
                        FileUtils.TryDeleteFile(filePath);

                    File.WriteAllText(filePath, json);
                    modelFilePath = filePath;

                    return modelFilePath;
                }
            }
            catch(Exception e)
            {
                _logger.Fatal(e);
                return string.Empty;
            }
        }

        public string RunSimulation(bool dragonfly, string modelFilePath)
        {
            var simulationSettings = AppSettings.Instance.StoredSettings.SimulationSettings;
            var simulationDir = Path.GetDirectoryName(modelFilePath);
            var simulationParamPath = PrepSimulation(simulationDir);

            var htmlReport = AppSettings.Instance.StoredSettings.SimulationSettings.HtmlReport;
            var osReport = AppSettings.Instance.StoredSettings.SimulationSettings.OpenStudioReport;
            var oswFilePath = htmlReport || osReport
                ? PrepOswSimulation(simulationDir, htmlReport, osReport)
                : string.Empty;

            return RunDragonflySimulateCommand(dragonfly, simulationDir, modelFilePath,
                simulationSettings.EpwFilePath, simulationParamPath, oswFilePath);
        }

        public string RunDragonflySimulateCommand(
            bool dragonfly, 
            string simulationDir, 
            string modelFilePath, 
            string epwFilePath, 
            string simulationParamPath, 
            string oswFilePath = "")
        {
            try
            {
                var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
                if (path == null)
                    return string.Empty;

                string pyPath = null;
                foreach (var p in path.Split(';'))
                {
                    var fullPath = Path.Combine(p, "python.exe");
                    if (!File.Exists(fullPath))
                        continue;

                    pyPath = fullPath;
                    break;
                }

                if (string.IsNullOrWhiteSpace(pyPath))
                    return string.Empty;

                var pyDir = Path.GetDirectoryName(pyPath);
                if (pyDir == null)
                    return string.Empty;

                var energyModel = dragonfly ? "dragonfly" : "honeybee";
                var dfPath = Path.Combine(pyDir, "Scripts", $"{energyModel}-energy.exe");
                if (!File.Exists(dfPath))
                    return string.Empty;

                var ps = PowerShell.Create();
                ps.AddCommand(pyPath)
                    .AddParameter("-m")
                    .AddCommand(dfPath)
                    .AddArgument("simulate")
                    .AddArgument("model")
                    .AddArgument("--folder")
                    .AddArgument(simulationDir);

                if (!string.IsNullOrWhiteSpace(oswFilePath))
                {
                    ps.AddArgument("--base-osw");
                    ps.AddArgument(oswFilePath);
                }

                ps.AddArgument("--sim-par-json")
                    .AddArgument(simulationParamPath)
                    .AddArgument(modelFilePath)
                    .AddArgument(epwFilePath);

                ps.Streams.Error.DataAdded += ErrorOnDataAdded;

                var outputCollection = new PSDataCollection<PSObject>();
                outputCollection.DataAdded += OutputCollectionOnDataAdded;
                var result = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);

                while (result.IsCompleted == false)
                {
                    Thread.Sleep(1000);
                }

                ps.Streams.Error.DataAdded -= ErrorOnDataAdded;

                return outputCollection.Any() ? "Success!" : "Failure!";
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                return "Failure!";
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
            catch (OperationCanceledException)
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

        public List<Shade> SelectFaces()
        {
            var result = new List<Shade>();
            try
            {
                var selection = UiDoc.Selection.PickObjects(ObjectType.Face, new FilterPlanarFace(Doc), "Please select Planar Faces only.");
                var shades = new List<Shade>();
                foreach (var tree in selection)
                {
                    var e = Doc.GetElement(tree);
                    var geometry = e.GetGeometryObjectFromReference(tree) as PlanarFace;
                    shades.Add(new Shade(new Face3D(geometry)));
                }

                return shades;
            }
            catch (OperationCanceledException)
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

        public List<Shade> SelectPlanting()
        {
            var result = new List<Shade>();
            try
            {
                var selection = UiDoc.Selection.PickObjects(ObjectType.Element, new FilterPlanting(), "Please select Planting only.");
                var shades = new List<Shade>();
                foreach (var reference in selection)
                {
                    shades.AddRange(ShadesFromTrees(Doc.GetElement(reference), Doc.ActiveView));
                }

                return shades;
            }
            catch (OperationCanceledException)
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

        public List<Shade> GetContextShades()
        {
            var shades = new List<Shade>();
            var trees = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_Planting).WhereElementIsNotElementType();
            var view = new FilteredElementCollector(Doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .FirstOrDefault(x => x.ViewType == ViewType.ThreeD && !x.IsTemplate);

            foreach (var tree in trees)
            {
                shades.AddRange(ShadesFromTrees(tree, view));
            }

            return shades;
        }

        #region Event Handlers

        private static void OutputCollectionOnDataAdded(object sender, DataAddedEventArgs e)
        {
            var record = ((PSDataCollection<PSObject>)sender)[e.Index];

            Messenger.Default.Send(new UpdateStatusBarMessage(record.BaseObject.ToString()));
        }

        private static void ErrorOnDataAdded(object sender, DataAddedEventArgs e)
        {
            var record = ((PSDataCollection<ErrorRecord>)sender)[e.Index];

            Messenger.Default.Send(new UpdateStatusBarMessage(record.Exception?.Message));
        }

        #endregion

        #region Utilities

        private List<HB.ProgramType> GetProgramTypeSet(IEnumerable<Room2D> rooms, ProgramType bProgramType)
        {
            var pTypes = rooms
                .GroupBy(x => x.Properties.Energy.ProgramType.Identifier)
                .Select(x => x.Key)
                .ToList();

            if (!pTypes.Contains(bProgramType.Identifier))
                pTypes.Add(bProgramType.Identifier);

            var jsonProgramTypes = RunHoneybeeEnergyCommand("program-types-by-id", pTypes);
            JsonConverter[] converters =
            {
                new HB.AnyOfJsonConverter()
            };

            return JsonConvert.DeserializeObject<List<HB.ProgramType>>(jsonProgramTypes, converters);
        }

        private HB.ConstructionSet GetDefaultConstructionSet()
        {
            var args = new List<string> { "Default Generic Construction Set", "--none-defaults", "False" };
            var jsonConstructionSets = RunHoneybeeEnergyCommand2("construction-set-by-id", args);
            JsonConverter[] converters =
            {
                new HB.AnyOfJsonConverter()
            };

            return JsonConvert.DeserializeObject<HB.ConstructionSet>(jsonConstructionSets, converters);
        }

        private List<HB.ConstructionSet> GetConstructionSets(IEnumerable<Room2D> rooms, ConstructionSet bConstructionSet)
        {
            var cSets = rooms
                .GroupBy(x => x.Properties.Energy.ConstructionSet.Identifier)
                .Select(x => x.Key)
                .ToList();

            if (!cSets.Contains(bConstructionSet.Identifier))
                cSets.Add(bConstructionSet.Identifier);

            var jsonConstructionSets = RunHoneybeeEnergyCommand("construction-sets-by-id", cSets);
            JsonConverter[] converters =
            {
                new HB.AnyOfJsonConverter()
            };

            return JsonConvert.DeserializeObject<List<HB.ConstructionSet>>(jsonConstructionSets, converters);
        }

        private static string PrepSimulation(string simulationDir)
        {
            var simParameter = new HB.SimulationParameter();
            var simParamPath = Path.Combine(simulationDir, "simulation_parameter.json");
            File.WriteAllText(simParamPath, simParameter.ToJson());

            return simParamPath;
        }

        private static string PrepOswSimulation(string simulationDir, bool htmlReport, bool openStudioReport)
        {
            var osw = new OpenStudioWorkflow();
            var measurePath = Path.Combine(HB.Helper.EnergyLibrary.LadybugToolsRootFolder, "resources", "measures");
            osw.MeasurePaths.Add(measurePath);

            if (htmlReport)
                osw.Steps.Add(new Step("OpenStudioResults"));

            if (openStudioReport)
                osw.Steps.Add(new Step("ViewData"));

            var jsonString = JsonConvert.SerializeObject(osw);
            var oswPath = Path.Combine(simulationDir, "workflow_user.osw");
            File.WriteAllText(oswPath, jsonString);

            return oswPath;
        }

        private static List<Shade> ShadesFromTrees(Element e, View v)
        {
            var bb = e.get_BoundingBox(v);
            var min = bb.Min;
            var max = bb.Max;
            var width = max.X - min.X;
            var depth = max.Y - min.Y;

            var plane1 = new List<Point3D>
            {
                new Point3D(new XYZ(min.X + (0.5 * width), min.Y, min.Z)),
                new Point3D(new XYZ(min.X + (0.5 * width), max.Y, min.Z)),
                new Point3D(new XYZ(min.X + (0.5 * width), max.Y, max.Z)),
                new Point3D(new XYZ(min.X + (0.5 * width), min.Y, max.Z))
            };
            var plane2 = new List<Point3D>
            {
                new Point3D(new XYZ(min.X, min.Y + (0.5 * depth), min.Z)),
                new Point3D(new XYZ(max.X, min.Y + (0.5 * depth), min.Z)),
                new Point3D(new XYZ(max.X, min.Y + (0.5 * depth), max.Z)),
                new Point3D(new XYZ(min.X, min.Y + (0.5 * depth), max.Z))
            };

            return new List<Shade>
            {
                new Shade(plane1), new Shade(plane2)
            };
        }

        private static void DF_AssignBoundaryConditions(IReadOnlyCollection<SpatialObjectWrapper> objects)
        {
            foreach (var so in objects)
            {
                var offset = so.Self.get_Parameter(BuiltInParameter.ROOM_LOWER_OFFSET).AsDouble();
                var boundaryCurves = so.Room2D.FloorBoundary.GetCurves(so.Level.Elevation + offset);
                for (var i = 0; i < boundaryCurves.Count; i++)
                {
                    var currentBc = so.Room2D.BoundaryConditions[i];
                    if (currentBc is Surface)
                        continue;

                    // (Konrad) Adiabatic can only be assigned to Boundaries that don't have a Window.
                    var allowAdiabatic = so.Room2D.WindowParameters[i] == null;
                    var bc = BoundaryConditionBase.DF_Init(objects.Where(x => !Equals(x, so) && x.Level.Id == so.Level.Id), boundaryCurves[i], so, allowAdiabatic);
                    if (bc is Outdoors)
                        continue;

                    so.Room2D.BoundaryConditions[i] = bc;
                }

                var holeCurves = so.Room2D.FloorHoles.SelectMany(x => x.GetCurves(so.Level.Elevation + offset)).ToList();
                for (var i = 0; i < holeCurves.Count; i++)
                {
                    var currentBc = so.Room2D.BoundaryConditions[boundaryCurves.Count + i];
                    if (currentBc is Surface)
                        continue;

                    // (Konrad) Adiabatic can only be assigned to Boundaries that don't have a Window.
                    var allowAdiabatic = so.Room2D.WindowParameters[boundaryCurves.Count + i] == null;
                    var bc = BoundaryConditionBase.DF_Init(objects.Where(x => !Equals(x, so) && x.Level.Id == so.Level.Id), holeCurves[i], so, allowAdiabatic);
                    if (bc is Outdoors)
                        continue;

                    so.Room2D.BoundaryConditions[boundaryCurves.Count + i] = bc;
                }
            }
        }

        private static void HB_AssignBoundaryConditions(IReadOnlyCollection<SpatialObjectWrapper> objects)
        {
            foreach (var so in objects)
            {
                var faces = so.Room2D.Faces;
                for (var i = 0; i < faces.Count; i++)
                {
                    var currentFace = so.Room2D.Faces[i];
                    currentFace.AssignBoundaryConditions(objects.Where(x => !Equals(x, so) && x.Level.Id == so.Level.Id));
                    //// (Konrad) Adiabatic can only be assigned to Boundaries that don't have a Window.
                    //var allowAdiabatic = currentFace.Apertures == null || currentFace.Apertures.Count <= 0;
                    //var bc = BoundaryConditionBase.HB_Init(objects.Where(x => !Equals(x, so) && x.Level.Id == so.Level.Id), faces[i], so, allowAdiabatic);

                    //currentFace.BoundaryCondition = bc;
                    //currentFace.Apertures?.ForEach(x => x.BoundaryCondition = bc);
                }
            }
        }

        #endregion
    }
}
