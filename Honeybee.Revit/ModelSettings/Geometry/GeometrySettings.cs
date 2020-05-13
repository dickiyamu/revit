using System.ComponentModel;

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
