using System.ComponentModel;

namespace Honeybee.Revit.ModelSettings.Simulation
{
    public class SimulationSettings : INotifyPropertyChanged
    {
        private string _epwFilePath;
        public string EpwFilePath
        {
            get { return _epwFilePath; }
            set { _epwFilePath = value; RaisePropertyChanged(nameof(EpwFilePath)); }
        }

        private string _simulationFolder;
        public string SimulationFolder
        {
            get { return _simulationFolder; }
            set { _simulationFolder = value; RaisePropertyChanged(nameof(SimulationFolder)); }
        }

        private bool _openStudioReport;
        public bool OpenStudioReport
        {
            get { return _openStudioReport; }
            set { _openStudioReport = value; RaisePropertyChanged(nameof(OpenStudioReport)); }
        }

        private bool _htmlReport;
        public bool HtmlReport
        {
            get { return _htmlReport; }
            set { _htmlReport = value; RaisePropertyChanged(nameof(HtmlReport)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
