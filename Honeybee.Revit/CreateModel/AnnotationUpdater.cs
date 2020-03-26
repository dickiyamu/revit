using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using GalaSoft.MvvmLight.Messaging;
using NLog;

namespace Honeybee.Revit.CreateModel
{
    public class AnnotationUpdater : IUpdater
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly UpdaterId _updaterId;
        private Dictionary<string, ElementId> ParameterIds { get; set; }
        
        public AnnotationUpdater(AddInId id)
        {
            _updaterId = new UpdaterId(id, new Guid("373aa82c-1e3b-4f08-bb2f-4a70c744ce11"));
        }

        public bool Register(Document doc)
        {
            var registered = false;
            try
            {
                if (!UpdaterRegistry.IsUpdaterRegistered(_updaterId, doc))
                {
                    UpdaterRegistry.RegisterUpdater(this, doc);
                    RefreshTriggers(doc);
                    registered = true;
                }
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
            }

            return registered;
        }

        public void RefreshTriggers(Document doc)
        {
            UpdaterRegistry.RemoveDocumentTriggers(_updaterId, doc);

            var catFilter = new ElementCategoryFilter(BuiltInCategory.OST_GenericAnnotation);
            UpdaterRegistry.AddTrigger(_updaterId, catFilter, Element.GetChangeTypeAny());
            UpdaterRegistry.AddTrigger(_updaterId, catFilter, Element.GetChangeTypeElementAddition());
            UpdaterRegistry.AddTrigger(_updaterId, catFilter, Element.GetChangeTypeElementDeletion());

            const string famName = "2020_BoundaryConditions";
            var annotation = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfClass(typeof(FamilyInstance))
                .WhereElementIsNotElementType()
                .FirstOrDefault(x => (doc.GetElement(x.GetTypeId()) as AnnotationSymbolType)?.FamilyName == famName);
            if (annotation == null) return;

            
            var paramId = annotation.LookupParameter("AdjacentRoom")?.Id;
            if (paramId == null) return;

            ParameterIds = new Dictionary<string, ElementId>
            {
                { "AdjacentRoom", paramId}
            };

            UpdaterRegistry.AddTrigger(_updaterId, catFilter, Element.GetChangeTypeParameter(paramId));
        }

        //public void Unregister(Document doc)
        //{
        //    try
        //    {
        //        if (UpdaterRegistry.IsUpdaterRegistered(_updaterId, doc))
        //        {
        //            UpdaterRegistry.UnregisterUpdater(_updaterId, doc);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Fatal(e);
        //    }
        //}

        public void Execute(UpdaterData data)
        {
            foreach (var id in data.GetModifiedElementIds())
            {
                foreach (var pId in ParameterIds)
                {
                    if (!data.IsChangeTriggered(id, Element.GetChangeTypeParameter(pId.Value))) continue;
                    if (!(data.GetDocument().GetElement(id) is FamilyInstance fi)) continue;

                    switch (pId.Key)
                    {
                        case "AdjacentRoom":
                            Messenger.Default.Send(new SurfaceAdjacentRoomChanged(new AnnotationWrapper(fi)));
                            break;
                        default:
                            break;
                    }
                }
            }

            //foreach (var id in data.GetDeletedElementIds())
            //{
            //    //TODO: Stop people from Deleting Annotations.
            //}
        }

        public UpdaterId GetUpdaterId()
        {
            return _updaterId;
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.FloorsRoofsStructuralWalls;
        }

        public string GetUpdaterName()
        {
            return "Project Information Updater.";
        }

        public string GetAdditionalInformation()
        {
            return "This updater is responsible for reporting changes to Project Name and Address.";
        }
    }
}
