using System;
using System.ComponentModel;
using Honeybee.Revit.ModelSettings.Geometry;
using Newtonsoft.Json;

namespace Honeybee.Revit
{
    public class StoredSettings : INotifyPropertyChanged
    {
        private GeometrySettings _geometrySettings = new GeometrySettings();
        public GeometrySettings GeometrySettings
        {
            get { return _geometrySettings; }
            set { _geometrySettings = value; RaisePropertyChanged(nameof(GeometrySettings)); }
        }

        [JsonConstructor]
        public StoredSettings()
        {
        }

        public string Serialize()
        {
            throw new NotImplementedException();
        }

        public static StoredSettings Deserialize(string json)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
