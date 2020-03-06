using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Honeybee.Core;
using Honeybee.Revit.CreateModel.Wrappers;
using NLog;

namespace Honeybee.Revit.CreateModel
{
    public class CreateModelRequestHandler : IExternalEventHandler
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public CreateModelRequest Request { get; } = new CreateModelRequest();
        public object Arg1 { get; set; }

        public void Execute(UIApplication app)
        {
            try
            {
                switch (Request.Take())
                {
                    case RequestId.None:
                    {
                        return;
                    }
                    case RequestId.ShowBoundaryConditions:
                        ShowBoundaryConditions(app);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                throw;
            }
        }

        public string GetName()
        {
            return "Create Model External Event";
        }

        private void ShowBoundaryConditions(UIApplication app)
        {
            if (!(Arg1 is SpatialObjectWrapper so)) return;

            var doc = app.ActiveUIDocument.Document;
            using (var trans = new Transaction(doc, "Create Filled Region"))
            {
                trans.Start();

                Cleanup(doc);
                CreateFilledRegion(doc, so);
                CreateAnnotations(doc, so);

                trans.Commit();
            }
        }

        private static void Cleanup(Document doc)
        {
            try
            {
                const string famName = "2020_BoundaryConditions";
                var existingAnnotations = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .OfClass(typeof(FamilyInstance))
                    .WhereElementIsNotElementType()
                    .Where(x => (doc.GetElement(x.GetTypeId()) as AnnotationSymbolType)?.FamilyName == famName)
                    .Select(x => x.Id)
                    .ToList();

                // TODO: (Konrad) Create our own Filled Region Type so that we can always find it.
                var existingRegions = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .OfClass(typeof(FilledRegion))
                    .WhereElementIsNotElementType()
                    .Where(x => (doc.GetElement(x.GetTypeId()) as FilledRegionType)?.Name == "Vertical")
                    .Select(x => x.Id)
                    .ToList();

                if (existingAnnotations.Any())
                    doc.Delete(existingAnnotations);
                if (existingRegions.Any())
                    doc.Delete(existingRegions);
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
            }
        }

        private static void CreateAnnotations(Document doc, SpatialObjectWrapper so)
        {
            try
            {
                const string famName = "2020_BoundaryConditions";
                var gaId = BuiltInCategory.OST_GenericAnnotation.GetHashCode();
                var loadedFam = new FilteredElementCollector(doc)
                    .OfClass(typeof(Family))
                    .Cast<Family>()
                    .FirstOrDefault(x => x.FamilyCategory.Id.IntegerValue == gaId && x.Name == famName);

                FamilySymbol fs = null;
                if (loadedFam == null)
                {
                    var resourcePath = GetFamilyFromResource(famName);
                    if (string.IsNullOrWhiteSpace(resourcePath) ||
                        !doc.LoadFamilySymbol(resourcePath, "Outdoors", out fs)) return;
                }
                else
                {
                    var symbols = loadedFam.GetFamilySymbolIds();
                    foreach (var id in symbols)
                    {
                        if (!(doc.GetElement(id) is FamilySymbol familySymbol) ||
                            familySymbol.Name != "Outdoors") continue;

                        fs = familySymbol;
                        break;
                    }
                }

                if (fs == null) return;
                if (!fs.IsActive) fs.Activate();

                foreach (var curve in so.Room2D.FloorBoundarySegments)
                {
                    var loc = curve.Evaluate(0.5, true);
                    doc.Create.NewFamilyInstance(loc, fs, doc.ActiveView);
                }
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
            }
        }

        private static void CreateFilledRegion(Document doc, SpatialObjectWrapper so)
        {
            try
            {
                var filledRegionType = new FilteredElementCollector(doc).OfClass(typeof(FilledRegionType)).FirstElementId();
                if (filledRegionType == null || filledRegionType == ElementId.InvalidElementId) return;

                var curves = new List<CurveLoop>();
                var loop = new CurveLoop();
                foreach (var curve in so.Room2D.FloorBoundarySegments)
                {
                    loop.Append(curve);
                }
                curves.Add(loop);

                FilledRegion.Create(doc, filledRegionType, doc.ActiveView.Id, curves);
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
            }
        }

        private static string GetFamilyFromResource(string resourceName)
        {
            var contents = Resources.StreamBinaryEmbeddedResource(Assembly.GetExecutingAssembly(),
                $"Honeybee.Honeybee.Revit.CreateModel.R20.{resourceName}.rfa");
            if (!contents.Any()) return string.Empty;

            var filePath = Path.Combine(Path.GetTempPath(), $"{resourceName}.rfa");

            // (Konrad) If file is locked (used by another process) it means it exists.
            // We can just return the file path to it. 
            if (File.Exists(filePath) && FileUtils.IsFileLocked(new FileInfo(filePath))) return filePath;

            using (var fileStream = File.Open(filePath, FileMode.Create))
            using (var writer = new BinaryWriter(fileStream))
            {
                writer.Write(contents);
            }

            if (!File.Exists(filePath) || new FileInfo(filePath).Length <= 0) return string.Empty;

            return filePath;
        }
    }

    public enum RequestId
    {
        None,
        ShowBoundaryConditions
    }

    public class CreateModelRequest
    {
        private int _request = (int)RequestId.None;

        public RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref _request, (int)RequestId.None);
        }

        public void Make(RequestId request)
        {
            Interlocked.Exchange(ref _request, (int)request);
        }
    }
}
