using System.ComponentModel;
using Newtonsoft.Json;

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
