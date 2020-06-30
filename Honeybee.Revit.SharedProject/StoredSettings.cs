using System.ComponentModel;
using Honeybee.Revit.CreateModel;
using Honeybee.Revit.ModelSettings.Geometry;
using Honeybee.Revit.ModelSettings.Simulation;
using Honeybee.Revit.Schemas.Converters;
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

        private EnergyModelSettings _energyModelSettings = new EnergyModelSettings();
        public EnergyModelSettings EnergyModelSettings
        {
            get { return _energyModelSettings; }
            set { _energyModelSettings = value; RaisePropertyChanged(nameof(EnergyModelSettings)); }
        }

        [JsonConstructor]
        public StoredSettings()
        {
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new Room2DSettingsConverter());
        }

        public static StoredSettings Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<StoredSettings>(json, new Room2DSettingsConverter());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
