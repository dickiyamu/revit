using System;
using System.ComponentModel;
using Honeybee.Revit.ModelSettings.Geometry;
using Honeybee.Revit.ModelSettings.Simulation;
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

        private SimulationSettings _simulationSettings = new SimulationSettings();
        public SimulationSettings SimulationSettings
        {
            get { return _simulationSettings; }
            set { _simulationSettings = value; RaisePropertyChanged(nameof(SimulationSettings)); }
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
