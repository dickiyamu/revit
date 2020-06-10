#region References

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Honeybee.Core.Extensions;
using Honeybee.Core.WPF;
using Honeybee.Revit.CreateModel.Wrappers;
using Honeybee.Revit.Schemas;
using Honeybee.Revit.Schemas.Honeybee;
using Path = System.IO.Path;
// ReSharper disable PossibleNullReferenceException

#endregion

namespace Honeybee.Revit.CreateModel
{
    public class CreateModelViewModel : ViewModelBase
    {
        #region Properties

        public RelayCommand<Window> Cancel { get; set; }
        public RelayCommand Help { get; set; }
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
        public RelayCommand ExportModel { get; set; }
        public RelayCommand RunSimulation { get; set; }
        public RelayCommand AddPlanting { get; set; }
        public RelayCommand AddFaces { get; set; }
        public RelayCommand ClearShades { get; set; }
        public RelayCommand ShowLog { get; set; }

        public CreateModelModel Model { get; set; }
        public ObservableCollection<SpatialObjectWrapper> SpatialObjectsModels { get; set; }
        public ListCollectionView SpatialObjects { get; set; }
        public SolidColorBrush BorderBrush { get; set; }
        public string Title { get; set; }

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

        private ObservableCollection<Shade> _contextShades = new ObservableCollection<Shade>();
        public ObservableCollection<Shade> ContextShades
        {
            get { return _contextShades; }
            set { _contextShades = value; RaisePropertyChanged(() => ContextShades); }
        }

        private bool _dragonfly;
        public bool Dragonfly
        {
            get { return _dragonfly; }
            set { _dragonfly = value; RaisePropertyChanged(() => Dragonfly); }
        }

        private AppSettings _settings;
        public AppSettings Settings
        {
            get { return _settings; }
            set { _settings = value; RaisePropertyChanged(() => Settings); }
        }

        #endregion

        public CreateModelViewModel(CreateModelModel model, bool dragonfly)
        {
            DispatcherHelper.Initialize();

            Model = model;
            Dragonfly = dragonfly;
            Title = Dragonfly ? "Dragonfly - Create Model" : "Honeybee - Create Model";
            Settings = AppSettings.Instance;
            ContextShades = Model.GetContextShades().ToObservableCollection();

            var color = dragonfly
                ? Color.FromRgb(0, 166, 81)
                : Color.FromRgb(245, 179, 76);
            BorderBrush = new SolidColorBrush(color);

            var so = Model.GetSpatialObjects();
            Levels = so.Select(x => x.Level).Distinct().ToObservableCollection();
            SpatialObjectsModels = so.ToObservableCollection();
            SpatialObjects = new ListCollectionView(SpatialObjectsModels);
            SpatialObjects.GroupDescriptions.Clear();
            SpatialObjects.GroupDescriptions.Add(new PropertyGroupDescription("Level", new LevelToNameConverter()));
            SpatialObjects.Filter = FilterDataGrid;

            Cancel = new RelayCommand<Window>(OnCancel);
            Help = new RelayCommand(OnHelp);
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
            ExportModel = new RelayCommand(OnExportModel);
            RunSimulation = new RelayCommand(OnRunSimulation);
            AddPlanting = new RelayCommand(OnAddPlanting);
            AddFaces = new RelayCommand(OnAddFaces);
            ClearShades = new RelayCommand(OnClearShades);
            ShowLog = new RelayCommand(OnShowLog);
        }

        #region Event Handlers

        private void OnExportModel()
        {
            ProcessExport();
        }

        private static void OnShowLog()
        {
            var simulationDir = AppSettings.Instance.StoredSettings.SimulationSettings.SimulationFolder;
            var logPath = Path.Combine(simulationDir, "simulation_log.txt");

            File.WriteAllLines(logPath, StatusBarManager.Logs);

            if (File.Exists(logPath))
                Process.Start(logPath);
        }

        private async void OnRunSimulation()
        {
            var modelName = Dragonfly ? "Dragonfly" : "Honeybee";
            StatusBarManager.InitializeProgress($"Exporting {modelName} Model...", 100, true);

            var modelPath = await Task.Run(() => ProcessExport());

            StatusBarManager.SetStatus("Start Simulation...");

            var result = await Task.Run(() => Model.RunSimulation(Dragonfly, modelPath));
            if (result == "Success!")
                ShowResults();

            StatusBarManager.FinalizeProgress(true);
            StatusBarManager.SetStatus(result);
            StatusBarManager.LogButton.Visibility = Visibility.Visible;
        }

        private void OnClearShades()
        {
            ContextShades.Clear();
        }

        private void OnAddFaces()
        {
            Model.SelectFaces().ForEach(x => ContextShades.Add(x));
        }

        private void OnAddPlanting()
        {
            Model.SelectPlanting().ForEach(x => ContextShades.Add(x));
        }

        private static void OnShowDetails(SpatialObjectWrapper so)
        {
            so.IsExpanded = !so.IsExpanded;
        }

        private void OnShowBoundaryConditions(SpatialObjectWrapper so)
        {
            Model.ShowBoundaryConditions(so);
        }

        private void OnResetConstructionSet()
        {
            foreach (SpatialObjectWrapper so in SpatialObjects)
            {
                if (!so.IsSelected)
                    continue;

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
                if (!so.IsSelected)
                    continue;

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
                if (!so.IsSelected)
                    continue;

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
                if (!so.IsSelected)
                    continue;

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
                if (so.IsConstructionSetOverriden)
                    continue;

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
                if (so.IsProgramTypeOverriden)
                    continue;

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
            if (!selected.Any())
                return;

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

        private static void OnHelp()
        {
            Process.Start("");
        }

        public void OnWindowLoaded()
        {
            Messenger.Default.Register<SurfaceAdjacentRoomChanged>(this, OnSurfaceAdjacentRoomChanged);
            Messenger.Default.Register<TypeChanged>(this, OnTypeChanged);
            Messenger.Default.Register<AnnotationsCreated>(this, OnAnnotationsCreated);
            Messenger.Default.Register<UpdateStatusBarMessage>(this, OnUpdateStatusBar);
        }

        public void OnWindowUnloaded()
        {
            Cleanup();
        }

        #endregion

        #region Message Handlers

        private static void OnUpdateStatusBar(UpdateStatusBarMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.Message))
                return;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                StatusBarManager.SetStatus(msg.Message);
                StatusBarManager.Logs.Add(msg.Message);
            });
        }

        private void OnAnnotationsCreated(AnnotationsCreated msg)
        {
            var index = SpatialObjectsModels.IndexOf(msg.SpatialObject);
            if (index == -1)
                return;

            SpatialObjectsModels[index] = msg.SpatialObject;
        }

        private void OnSurfaceAdjacentRoomChanged(SurfaceAdjacentRoomChanged msg)
        {
            // (Konrad) Find the Object Wrapper with matching Id.
            var found = SpatialObjectsModels.FirstOrDefault(x =>
                x.Room2D.Annotations.FirstOrDefault(y => y.UniqueId == msg.Annotation.UniqueId) != null);
            if (found == null)
                return;

            var index = found.Room2D.Annotations.IndexOf(msg.Annotation);
            if (!(found.Room2D.BoundaryConditions[index] is Surface bc))
                return;

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

        private void ShowResults()
        {
            if (Dragonfly)
            {
                var osmPath = StatusBarManager.Logs.FirstOrDefault(x => x.EndsWith(".sql"));
                var rootDir = Path.GetDirectoryName(osmPath);
                if (!Directory.Exists(rootDir))
                    return;

                if (AppSettings.Instance.StoredSettings.SimulationSettings.OpenStudioReport)
                {
                    var osReportPath = Path.Combine(rootDir, "002_OpenStudioResults\\report.html");
                    Process.Start(osReportPath);
                }

                if (AppSettings.Instance.StoredSettings.SimulationSettings.HtmlReport)
                {
                    var htmlReportPath = Path.Combine(rootDir, "003_ViewData\\report.html");
                    Process.Start(htmlReportPath);
                }
            }
            else
            {
                var simulationDir = AppSettings.Instance.StoredSettings.SimulationSettings.SimulationFolder;

                if (AppSettings.Instance.StoredSettings.SimulationSettings.OpenStudioReport)
                {
                    var osReportPath = Path.Combine(simulationDir, "reports\\openstudio_results_report.html");
                    Process.Start(osReportPath);
                }

                if (AppSettings.Instance.StoredSettings.SimulationSettings.HtmlReport)
                {
                    var htmlReportPath = Path.Combine(simulationDir, "reports\\view_data_report.html");
                    Process.Start(htmlReportPath);
                }
            }
        }

        private string ProcessExport()
        {
            var selected = SpatialObjects.SourceCollection.Cast<SpatialObjectWrapper>()
                .Where(x => x.IsSelected)
                .Select(x => x.Room2D)
                .ToList();

            if (selected.Any())
            {
                var modelPath = Model.SerializeRoom2D(selected, BldgProgramType, BldgConstructionSet, Dragonfly, ContextShades.ToList());
                return modelPath;
            }

            return null;
        }

        private bool FilterDataGrid(object obj)
        {
            var so = obj as SpatialObjectWrapper;
            if (so != null && (ShowRooms && so.ObjectType == SpatialObjectType.Room))
                return true;

            if (so != null && (ShowSpaces && so.ObjectType == SpatialObjectType.Space))
                return true;

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
