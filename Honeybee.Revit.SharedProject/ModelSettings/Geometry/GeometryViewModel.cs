using GalaSoft.MvvmLight;

namespace Honeybee.Revit.ModelSettings.Geometry
{
    public class GeometryViewModel : ViewModelBase
    {
        private AppSettings _settings;
        public AppSettings Settings
        {
            get { return _settings; }
            set { _settings = value; RaisePropertyChanged(() => Settings); }
        }

        public GeometryViewModel()
        {
            Settings = AppSettings.Instance;
        }
    }
}
