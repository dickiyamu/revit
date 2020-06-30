using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Honeybee.Core.WPF;
using Honeybee.Revit.ModelSettings.Geometry;
using Honeybee.Revit.ModelSettings.Simulation;

namespace Honeybee.Revit.ModelSettings
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsModel Model { get; set; }
        public RelayCommand<Window> WindowLoaded { get; set; }
        public RelayCommand<Window> Close { get; set; }
        public RelayCommand Help { get; set; }

        public ObservableCollection<TabItem> TabItems { get; set; }

        private int _selectedTab;
        public int SelectedTab
        {
            get { return _selectedTab; }
            set { _selectedTab = value; RaisePropertyChanged(() => SelectedTab); }
        }

        public SettingsViewModel(SettingsModel model, int selectedTab = 0)
        {
            Model = model;
            WindowLoaded = new RelayCommand<Window>(OnWindowLoaded);
            Close = new RelayCommand<Window>(OnClose);
            Help = new RelayCommand(OnHelp);

            TabItems = new ObservableCollection<TabItem>
            {
                new TabItem
                {
                    Content = new GeometryControl {DataContext = new GeometryViewModel(new GeometryModel(Model.UiDoc))},
                    Header = "Geometry"
                },
                new TabItem
                {
                    Content = new SimulationControl {DataContext = new SimulationViewModel()},
                    Header = "Simulation"
                }
            };

            SelectedTab = selectedTab;
        }

        private static void OnHelp()
        {
        }

        private static void OnClose(Window win)
        {
            win.Close();
        }

        private static void OnWindowLoaded(Window win)
        {
            StatusBarManager.ProgressBar = ((SettingsView)win).ProgressBar;
            StatusBarManager.StatusLabel = ((SettingsView)win).StatusLabel;
        }
    }
}
