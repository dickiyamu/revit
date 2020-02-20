using System.ComponentModel;
using Newtonsoft.Json;
using NLog.LayoutRenderers;

namespace Honeybee.Revit.Schemas
{
    public class Room2DEnergyPropertiesAbridged : INotifyPropertyChanged
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return GetType().Name; }
        }

        private ProgramType _programType = new ProgramType();
        private ConstructionSet _constructionSet = new ConstructionSet();
        private HvacTypes _hvac;
        private bool _conditioned;

        [JsonProperty("program_type")]
        public ProgramType ProgramType
        {
            get { return _programType; }
            set { _programType = value; RaisePropertyChanged(nameof(ProgramType)); }
        }

        [JsonProperty("construction_set")]
        public ConstructionSet ConstructionSet
        {
            get { return _constructionSet; }
            set { _constructionSet = value; RaisePropertyChanged(nameof(ConstructionSet)); }
        }

        [JsonIgnore]
        public bool Conditioned
        {
            get { return _conditioned; }
            set { _conditioned = value; RaisePropertyChanged(nameof(Conditioned)); }
        }

        [JsonProperty("hvac")]
        public HvacTypes Hvac
        {
            get { return _hvac; }
            set { _hvac = value; RaisePropertyChanged(nameof(Hvac)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
