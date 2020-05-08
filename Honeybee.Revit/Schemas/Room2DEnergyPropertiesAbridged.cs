using System.ComponentModel;
using Newtonsoft.Json;
using DF = DragonflySchema;
using HB = HoneybeeSchema;

namespace Honeybee.Revit.Schemas
{
    public class Room2DEnergyPropertiesAbridged : ISchema<DF.Room2DEnergyPropertiesAbridged, HB.RoomEnergyPropertiesAbridged>, INotifyPropertyChanged
    {
        public string Type
        {
            get { return GetType().Name; }
        }

        private ProgramType _programType = new ProgramType();
        [JsonProperty("program_type")]
        public ProgramType ProgramType
        {
            get { return _programType; }
            set { _programType = value; RaisePropertyChanged(nameof(ProgramType)); }
        }

        private ConstructionSet _constructionSet = new ConstructionSet();
        [JsonProperty("construction_set")]
        public ConstructionSet ConstructionSet
        {
            get { return _constructionSet; }
            set { _constructionSet = value; RaisePropertyChanged(nameof(ConstructionSet)); }
        }

        private bool _conditioned;
        [JsonIgnore]
        public bool Conditioned
        {
            get { return _conditioned; }
            set { _conditioned = value; RaisePropertyChanged(nameof(Conditioned)); }
        }

        private HvacTypes _hvac;
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

        public DF.Room2DEnergyPropertiesAbridged ToDragonfly()
        {
            return new DF.Room2DEnergyPropertiesAbridged(ConstructionSet.Identifier, ProgramType.Identifier, Hvac?.Name);
        }

        public HB.RoomEnergyPropertiesAbridged ToHoneybee()
        {
            return new HB.RoomEnergyPropertiesAbridged(
                ConstructionSet.Identifier, 
                ProgramType.Identifier, 
                Hvac?.Name, 
                null, // people
                null, // lighting
                null, // electrical equipment
                null, // gas equipment
                null, // infiltration
                null, // ventilation
                null); // setpoint
        }
    }
}
