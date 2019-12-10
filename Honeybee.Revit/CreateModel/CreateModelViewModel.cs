#region References

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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

        private ListCollectionView _spatialObjects = new ListCollectionView(new List<SpatialObjectWrapper>());
        public ListCollectionView SpatialObjects
        {
            get { return _spatialObjects; }
            set { _spatialObjects = value; RaisePropertyChanged(() => SpatialObjects); }
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

        private ProgramType _bldgProgramType = new ProgramType();
        public ProgramType BldgProgramType
        {
            get { return _bldgProgramType; }
            set { _bldgProgramType = value; RaisePropertyChanged(() => BldgProgramType); }
        }

        #endregion

        public CreateModelViewModel(CreateModelModel model)
        {
            Model = model;
            var so = Model.GetSpatialObjects();

            Levels = so.Select(x => x.Level).Distinct().ToObservableCollection();
            SpatialObjects = new ListCollectionView(so);
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
        }
        
        #region Event Handlers

        private void OnResetConstructionSet()
        {
            foreach (SpatialObjectWrapper so in SpatialObjects)
            {
                if (!so.IsSelected) continue;

                so.ConstructionSet.Vintage = BldgConstructionSet.Vintage;
                so.ConstructionSet.ClimateZone = BldgConstructionSet.ClimateZone;
                so.ConstructionSet.ConstructionType = BldgConstructionSet.ConstructionType;
                so.IsConstructionSetOverriden = false;
            }
        }

        private void OnResetProgramType()
        {
            foreach (SpatialObjectWrapper so in SpatialObjects)
            {
                if (!so.IsSelected) continue;

                so.ProgramType.Vintage = BldgProgramType.Vintage;
                so.ProgramType.BuildingProgram = BldgProgramType.BuildingProgram;
                so.ProgramType.RoomProgram = BldgProgramType.RoomProgram;
                so.IsProgramTypeOverriden = false;
            }
        }

        private void OnClosePopup(Popup popup)
        {
            popup.IsOpen = false;

            foreach (SpatialObjectWrapper so in SpatialObjects)
            {
                if (!so.IsSelected) continue;

                so.ConstructionSet.Vintage = ConstructionSetTemp.Vintage;
                so.ConstructionSet.ClimateZone = ConstructionSetTemp.ClimateZone;
                so.ConstructionSet.ConstructionType = ConstructionSetTemp.ConstructionType;
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

                so.ProgramType.Vintage = ProgramTypeTemp.Vintage;
                so.ProgramType.BuildingProgram = ProgramTypeTemp.BuildingProgram;
                so.ProgramType.RoomProgram = ProgramTypeTemp.RoomProgram;
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

                so.ConstructionSet.Vintage = BldgConstructionSet.Vintage;
                so.ConstructionSet.ClimateZone = BldgConstructionSet.ClimateZone;
                so.ConstructionSet.ConstructionType = BldgConstructionSet.ConstructionType;
            }
        }

        private void OnCloseBuildingProgramTypePopup(Popup popup)
        {
            popup.IsOpen = false;

            foreach (SpatialObjectWrapper so in SpatialObjects)
            {
                if (so.IsProgramTypeOverriden) continue;

                so.ProgramType.Vintage = BldgProgramType.Vintage;
                so.ProgramType.BuildingProgram = BldgProgramType.BuildingProgram;
                so.ProgramType.RoomProgram = BldgProgramType.RoomProgram;
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

        private static void OnOk(Window win)
        {
            win.Close();
        }

        private static void OnHelp()
        {
            Process.Start("");
        }

        private static void OnWindowLoaded(Window win)
        {
            StatusBarManager.ProgressBar = ((CreateModelView)win).ProgressBar;
            StatusBarManager.StatusLabel = ((CreateModelView)win).StatusLabel;
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
