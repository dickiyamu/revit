using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Honeybee.Core.Extensions;
using Honeybee.Core.WPF;
using Honeybee.Revit.CreateModel.Wrappers;

namespace Honeybee.Revit.CreateModel
{
    public class CreateModelViewModel : ViewModelBase
    {
        public CreateModelModel Model { get; set; }
        public RelayCommand<Window> Cancel { get; set; }
        public RelayCommand<Window> Ok { get; set; }
        public RelayCommand Help { get; set; }
        public RelayCommand<Window> WindowLoaded { get; set; }
        public RelayCommand ClearFilters { get; set; }
        public RelayCommand FilterChanged { get; set; }
        public RelayCommand PickSpatialObjects { get; set; }

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
        }

        private void OnPickSpatialObjects()
        {
            var selected = Model.SelectRoomsSpaces();
            if (!selected.Any()) return;

            SpatialObjects.SourceCollection.Cast<SpatialObjectWrapper>().ForEach(x => x.IsSelected = selected.Contains(x));
        }

        public bool FilterDataGrid(object obj)
        {
            var so = obj as SpatialObjectWrapper;
            if (so != null && (ShowRooms && so.ObjectType == SpatialObjectType.Room)) return true;
            if (so != null && (ShowSpaces && so.ObjectType == SpatialObjectType.Space)) return true;

            return false;
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
            win.Close();
        }

        private static void OnHelp()
        {
            Process.Start("");
        }

        private void OnWindowLoaded(Window win)
        {
            StatusBarManager.ProgressBar = ((CreateModelView)win).ProgressBar;
            StatusBarManager.StatusLabel = ((CreateModelView)win).StatusLabel;
        }
    }

    public enum FilterType
    {
        None,
        Level
    }
}
