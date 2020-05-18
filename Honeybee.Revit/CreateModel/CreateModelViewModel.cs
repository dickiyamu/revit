#region References

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Honeybee.Core.Extensions;
using Honeybee.Core.WPF;
using Honeybee.Revit.CreateModel.Wrappers;
using Honeybee.Revit.Schemas;

#endregion

namespace Honeybee.Revit.CreateModel
{
    public class CreateModelViewModel : ViewModelBase
    {
        #region Properties

        public CreateModelModel Model { get; set; }
        public RelayCommand<Window> Cancel { get; set; }
        public RelayCommand<Window> Ok { get; set; }
        public RelayCommand Help { get; set; }
        public RelayCommand<Window> WindowLoaded { get; set; }
        public RelayCommand ClearFilters { get; set; }
        public RelayCommand FilterChanged { get; set; }
        public RelayCommand PickSpatialObjects { get; set; }
        public RelayCommand<Popup> OpenPopup { get; set; }
        public RelayCommand<Popup> ClosePopup { get; set; }
        public RelayCommand<Popup> CloseBldgConstructionSetPopup { get; set; }
        public RelayCommand<Popup> CloseProgramTypePopup { get; set; }
        public RelayCommand<Popup> CloseBuildingProgramTypePopup { get; set; }
        public RelayCommand ResetConstructionSet { get; set; }
        public RelayCommand ResetProgramType { get; set; }
        public RelayCommand<SpatialObjectWrapper> ShowBoundaryConditions { get; set; }
        public RelayCommand<SpatialObjectWrapper> ShowDetails { get; set; }
        public RelayCommand<Window> ExportHoneybee { get; set; }
        public RelayCommand<Window> ExportDragonfly { get; set; }

        public ObservableCollection<SpatialObjectWrapper> SpatialObjectsModels { get; set; }
        public ListCollectionView SpatialObjects { get; set; }

        private bool _isExpanded = true;
        // ReSharper disable once UnusedMember.Global
        public bool IsExpanded
        {
            get
            {
                if (_isExpanded)
                {
                    _isExpanded = false;
                    return true;
                }
                return false;
            }
        }

        private FilterType _filterType = FilterType.None;
        public FilterType FilterType
        {
            get { return _filterType; }
            set { _filterType = value; RaisePropertyChanged(() => FilterType); }
        }

        private ObservableCollection<LevelWrapper> _levels = new ObservableCollection<LevelWrapper>();
        public ObservableCollection<LevelWrapper> Levels
        {
            get { return _levels; }
            set { _levels = value; RaisePropertyChanged(() => Levels); }
        }

        private LevelWrapper _selectedLevel;
        public LevelWrapper SelectedLevel
        {
            get { return _selectedLevel; }
            set { _selectedLevel = value; RaisePropertyChanged(() => SelectedLevel); }
        }

        private bool _showRooms = true;
        public bool ShowRooms
        {
            get { return _showRooms; }
            set
            {
                _showRooms = value;
                SpatialObjects.Filter = null;
                SpatialObjects.Filter = FilterDataGrid;
                RaisePropertyChanged(() => ShowRooms);
            }
        }

        private bool _showSpaces;
        public bool ShowSpaces
        {
            get { return _showSpaces; }
            set
            {
                _showSpaces = value;
                SpatialObjects.Filter = null;
                SpatialObjects.Filter = FilterDataGrid;
                RaisePropertyChanged(() => ShowSpaces);
            }
        }

        private ConstructionSet _bldgConstructionSet = new ConstructionSet();
        public ConstructionSet BldgConstructionSet
        {
            get { return _bldgConstructionSet; }
            set { _bldgConstructionSet = value; RaisePropertyChanged(() => BldgConstructionSet); }
        }

        private ProgramType _bldgProgramType = new ProgramType();
        public ProgramType BldgProgramType
        {
            get { return _bldgProgramType; }
            set { _bldgProgramType = value; RaisePropertyChanged(() => BldgProgramType); }
        }

        private ConstructionSet _constructionSetTemp = new ConstructionSet();
        public ConstructionSet ConstructionSetTemp
        {
            get { return _constructionSetTemp; }
            set { _constructionSetTemp = value; RaisePropertyChanged(() => ConstructionSetTemp); }
        }

        private ProgramType _programTypeTemp = new ProgramType();
        public ProgramType ProgramTypeTemp
        {
            get { return _programTypeTemp; }
            set { _programTypeTemp = value; RaisePropertyChanged(() => ProgramTypeTemp); }
        }

        #endregion

        public CreateModelViewModel(CreateModelModel model)
        {
            Model = model;

            var so = Model.GetSpatialObjects();
            Levels = so.Select(x => x.Level).Distinct().ToObservableCollection();
            SpatialObjectsModels = so.ToObservableCollection();
            SpatialObjects = new ListCollectionView(SpatialObjectsModels);
            SpatialObjects.GroupDescriptions.Clear();
            SpatialObjects.GroupDescriptions.Add(new PropertyGroupDescription("Level", new LevelToNameConverter()));
            SpatialObjects.Filter = FilterDataGrid;

            Cancel = new RelayCommand<Window>(OnCancel);
            Ok = new RelayCommand<Window>(OnOk);
            Help = new RelayCommand(OnHelp);
            WindowLoaded = new RelayCommand<Window>(OnWindowLoaded);
            ClearFilters = new RelayCommand(OnClearFilters);
            FilterChanged = new RelayCommand(OnFilterChanged);
            PickSpatialObjects = new RelayCommand(OnPickSpatialObjects);
            OpenPopup = new RelayCommand<Popup>(OnOpenPopup);
            ClosePopup = new RelayCommand<Popup>(OnClosePopup);
            CloseBldgConstructionSetPopup = new RelayCommand<Popup>(OnCloseBldgConstructionSetPopup);
            CloseProgramTypePopup = new RelayCommand<Popup>(OnCloseProgramTypePopup);
            CloseBuildingProgramTypePopup = new RelayCommand<Popup>(OnCloseBuildingProgramTypePopup);
            ResetConstructionSet = new RelayCommand(OnResetConstructionSet);
            ResetProgramType = new RelayCommand(OnResetProgramType);
            ShowBoundaryConditions = new RelayCommand<SpatialObjectWrapper>(OnShowBoundaryConditions);
            ShowDetails = new RelayCommand<SpatialObjectWrapper>(OnShowDetails);
            ExportHoneybee = new RelayCommand<Window>(OnExportHoneybee);
            ExportDragonfly = new RelayCommand<Window>(OnExportDragonfly);
        }

        private void OnExportDragonfly(Window win)
        {
            var selected = SpatialObjects.SourceCollection.Cast<SpatialObjectWrapper>()
                .Where(x => x.IsSelected)
                .Select(x => x.Room2D)
                .ToList();
            if (selected.Any()) Model.SerializeRoom2D(selected, BldgProgramType, BldgConstructionSet);

            win.Close();
        }

        private void OnExportHoneybee(Window win)
        {
            var selected = SpatialObjects.SourceCollection.Cast<SpatialObjectWrapper>()
                .Where(x => x.IsSelected)
                .Select(x => x.Room2D)
                .ToList();
            if (selected.Any()) Model.SerializeRoom2D(selected, BldgProgramType, BldgConstructionSet, false);

            //win.Close();
        }

        private static void OnShowDetails(SpatialObjectWrapper so)
        {
            so.IsExpanded = !so.IsExpanded;
        }

        #region Event Handlers

        private void OnShowBoundaryConditions(SpatialObjectWrapper so)
        {
            Model.ShowBoundaryConditions(so);
        }

        private void OnResetConstructionSet()
        {
            foreach (SpatialObjectWrapper so in SpatialObjects)
            {
                if (!so.IsSelected) continue;

                so.Room2D.Properties.Energy.ConstructionSet.Vintage = BldgConstructionSet.Vintage;
                so.Room2D.Properties.Energy.ConstructionSet.ClimateZone = BldgConstructionSet.ClimateZone;
                so.Room2D.Properties.Energy.ConstructionSet.ConstructionType = BldgConstructionSet.ConstructionType;
                so.IsConstructionSetOverriden = false;
            }
        }

        private void OnResetProgramType()
        {
            foreach (SpatialObjectWrapper so in SpatialObjects)
            {
                if (!so.IsSelected) continue;

                so.Room2D.Properties.Energy.ProgramType.Vintage = BldgProgramType.Vintage;
                so.Room2D.Properties.Energy.ProgramType.BuildingProgram = BldgProgramType.BuildingProgram;
                so.Room2D.Properties.Energy.ProgramType.RoomProgram = BldgProgramType.RoomProgram;
                so.IsProgramTypeOverriden = false;
            }
        }

        private void OnClosePopup(Popup popup)
        {
            popup.IsOpen = false;

            foreach (SpatialObjectWrapper so in SpatialObjects)
            {
                if (!so.IsSelected) continue;

                so.Room2D.Properties.Energy.ConstructionSet.Vintage = ConstructionSetTemp.Vintage;
                so.Room2D.Properties.Energy.ConstructionSet.ClimateZone = ConstructionSetTemp.ClimateZone;
                so.Room2D.Properties.Energy.ConstructionSet.ConstructionType = ConstructionSetTemp.ConstructionType;
                so.IsConstructionSetOverriden = true;
            }

            ConstructionSetTemp = new ConstructionSet();
        }

        private void OnCloseProgramTypePopup(Popup popup)
        {
            popup.IsOpen = false;

            foreach (SpatialObjectWrapper so in SpatialObjects)
            {
                if (!so.IsSelected) continue;

                so.Room2D.Properties.Energy.ProgramType.Vintage = ProgramTypeTemp.Vintage;
                so.Room2D.Properties.Energy.ProgramType.BuildingProgram = ProgramTypeTemp.BuildingProgram;
                so.Room2D.Properties.Energy.ProgramType.RoomProgram = ProgramTypeTemp.RoomProgram;
                so.IsProgramTypeOverriden = true;
            }
            
            ProgramTypeTemp = new ProgramType();
        }

        private void OnCloseBldgConstructionSetPopup(Popup popup)
        {
            popup.IsOpen = false;

            foreach (SpatialObjectWrapper so in SpatialObjects)
            {
                if (so.IsConstructionSetOverriden) continue;

                so.Room2D.Properties.Energy.ConstructionSet.Vintage = BldgConstructionSet.Vintage;
                so.Room2D.Properties.Energy.ConstructionSet.ClimateZone = BldgConstructionSet.ClimateZone;
                so.Room2D.Properties.Energy.ConstructionSet.ConstructionType = BldgConstructionSet.ConstructionType;
            }
        }

        private void OnCloseBuildingProgramTypePopup(Popup popup)
        {
            popup.IsOpen = false;

            foreach (SpatialObjectWrapper so in SpatialObjects)
            {
                if (so.IsProgramTypeOverriden) continue;

                so.Room2D.Properties.Energy.ProgramType.Vintage = BldgProgramType.Vintage;
                so.Room2D.Properties.Energy.ProgramType.BuildingProgram = BldgProgramType.BuildingProgram;
                so.Room2D.Properties.Energy.ProgramType.RoomProgram = BldgProgramType.RoomProgram;
            }
        }

        private static void OnOpenPopup(Popup popup)
        {
            popup.IsOpen = true;
        }

        private void OnPickSpatialObjects()
        {
            var selected = Model.SelectRoomsSpaces();
            if (!selected.Any()) return;

            //Model.WriteJournalComment($"Selected Rooms/Spaces: {string.Join(":::", selected.Select(x => x.Name))}.");
            SpatialObjects.SourceCollection.Cast<SpatialObjectWrapper>().ForEach(x => x.IsSelected = selected.Contains(x));
        }

        private void OnFilterChanged()
        {
            SpatialObjects.SourceCollection.Cast<SpatialObjectWrapper>().ForEach(x => x.IsSelected = Equals(x.Level, SelectedLevel));
        }

        private void OnClearFilters()
        {
            FilterType = FilterType.None;

            SpatialObjects.SourceCollection.Cast<SpatialObjectWrapper>().ForEach(x => x.IsSelected = false);
        }

        private static void OnCancel(Window win)
        {
            win.Close();
        }

        private void OnOk(Window win)
        {
            //var selected = SpatialObjects.SourceCollection.Cast<SpatialObjectWrapper>()
            //    .Where(x => x.IsSelected)
            //    .Select(x => x.Room2D)
            //    .ToList();
            //if (selected.Any()) Model.SerializeRoom2D(selected);

            //win.Close();
        }

        private static void OnHelp()
        {
            Process.Start("");
        }

        private void OnWindowLoaded(Window win)
        {
            StatusBarManager.ProgressBar = ((CreateModelView)win).ProgressBar;
            StatusBarManager.StatusLabel = ((CreateModelView)win).StatusLabel;

            Messenger.Default.Register<SurfaceAdjacentRoomChanged>(this, OnSurfaceAdjacentRoomChanged);
            Messenger.Default.Register<TypeChanged>(this, OnTypeChanged);
            Messenger.Default.Register<AnnotationsCreated>(this, OnAnnotationsCreated);
        }

        #endregion

        #region Message Handlers

        private void OnAnnotationsCreated(AnnotationsCreated msg)
        {
            var index = SpatialObjectsModels.IndexOf(msg.SpatialObject);
            if (index == -1) return;

            SpatialObjectsModels[index] = msg.SpatialObject;
        }

        private void OnSurfaceAdjacentRoomChanged(SurfaceAdjacentRoomChanged msg)
        {
            // (Konrad) Find the Object Wrapper with matching Id.
            var found = SpatialObjectsModels.FirstOrDefault(x =>
                x.Room2D.Annotations.FirstOrDefault(y => y.UniqueId == msg.Annotation.UniqueId) != null);
            if (found == null) return;

            var index = found.Room2D.Annotations.IndexOf(msg.Annotation);
            if (!(found.Room2D.BoundaryConditions[index] is Surface bc)) return;

            // (Konrad) We only need to update the BoundaryCondition Adjacent Room properties.
            var newBc = new Tuple<string, string, string>(string.Empty, bc.BoundaryConditionObjects.Item1, msg.Annotation.AdjacentRoom);
            bc.BoundaryConditionObjects = newBc;

            // (Konrad) Replace Annotation with new one.
            found.Room2D.Annotations[index] = msg.Annotation;
        }

        private void OnTypeChanged(TypeChanged msg)
        {
            // (Konrad) Find the Object Wrapper with matching Id.
            var found = SpatialObjectsModels.FirstOrDefault(x =>
                x.Room2D.Annotations.FirstOrDefault(y => y.UniqueId == msg.Annotation.UniqueId) != null);
            if (found == null) return;

            var index = found.Room2D.Annotations.IndexOf(msg.Annotation);

            // (Konrad) Type changed so we need a new Boundary Condition.
            BoundaryConditionBase newBc;
            switch (msg.Annotation.FamilySymbolName)
            {
                case "Ground":
                    newBc = new Ground();
                    break;
                case "Surface":
                    newBc = new Surface(new Tuple<string, string, string>(string.Empty, "-1", string.Empty));
                    break;
                case "Outdoors":
                    newBc = new Outdoors();
                    break;
                case "Adiabatic":
                    newBc = new Adiabatic();
                    break;
                default:
                    newBc = new Outdoors();
                    break;
            }

            // (Konrad) Update Boundary Condition and Annotation references.
            found.Room2D.BoundaryConditions[index] = newBc;
            found.Room2D.Annotations[index] = msg.Annotation;
        }

        #endregion

        #region Utilities

        private bool FilterDataGrid(object obj)
        {
            var so = obj as SpatialObjectWrapper;
            if (so != null && (ShowRooms && so.ObjectType == SpatialObjectType.Room)) return true;
            if (so != null && (ShowSpaces && so.ObjectType == SpatialObjectType.Space)) return true;

            return false;
        }

        #endregion
    }

    public enum FilterType
    {
        None,
        Level
    }
}
