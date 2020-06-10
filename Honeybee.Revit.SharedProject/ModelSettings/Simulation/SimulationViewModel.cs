using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Honeybee.Core.WPF;

namespace Honeybee.Revit.ModelSettings.Simulation
{
    public class SimulationViewModel : ViewModelBase
    {
        public RelayCommand<TextBox> SelectEpwFile { get; set; }
        public RelayCommand<TextBox> SelectSimulationFolder { get; set; }

        private AppSettings _settings;
        public AppSettings Settings
        {
            get { return _settings; }
            set { _settings = value; RaisePropertyChanged(() => Settings); }
        }

        public SimulationViewModel()
        {
            Settings = AppSettings.Instance;

            SelectEpwFile = new RelayCommand<TextBox>(OnSelectEpwFile);
            SelectSimulationFolder = new RelayCommand<TextBox>(OnSelectSimulationFolder);
        }

        private static void OnSelectSimulationFolder(TextBox tb)
        {
            var dirPath = Dialogs.SelectDirectory();
            tb.Text = dirPath;
        }

        private static void OnSelectEpwFile(TextBox tb)
        {
            var epwFilePath = Dialogs.SelectFile("Weather File (*.epw)|*.epw", ".epw", "Honeybee - Select Weather File");
            tb.Text = epwFilePath;
        }
    }
}
