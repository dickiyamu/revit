using System.Collections.ObjectModel;
using System.ComponentModel;
using Honeybee.Revit.ModelSettings.Geometry.Wrappers;

namespace Honeybee.Revit.ModelSettings.Geometry
{
    public class GeometrySettings : INotifyPropertyChanged
    {
        private bool _pullUpRoomHeight;
        public bool PullUpRoomHeight
        {
            get { return _pullUpRoomHeight; }
            set { _pullUpRoomHeight = value; RaisePropertyChanged(nameof(PullUpRoomHeight)); }
        }

        private double _tolerance = 0.01;
        public double Tolerance
        {
            get { return _tolerance; }
            set { _tolerance = value; RaisePropertyChanged(nameof(Tolerance)); }
        }

        private ObservableCollection<GlazingTypeWrapper> _glazingTypes = new ObservableCollection<GlazingTypeWrapper>();
        public ObservableCollection<GlazingTypeWrapper> GlazingTypes
        {
            get { return _glazingTypes; }
            set { _glazingTypes = value; RaisePropertyChanged(nameof(GlazingTypes)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
