using System.ComponentModel;
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
        private ConstructionSet _constructionSet = new ConstructionSet();
        private HvacTypes _hvac;
        private bool _conditioned;


        public ProgramType ProgramType
        {
            get { return _programType; }
            set { _programType = value; RaisePropertyChanged(nameof(ProgramType)); }
        }

        public ConstructionSet ConstructionSet
        {
            get { return _constructionSet; }
            set { _constructionSet = value; RaisePropertyChanged(nameof(ConstructionSet)); }
        }

        public bool Conditioned
        {
            get { return _conditioned; }
            set { _conditioned = value; RaisePropertyChanged(nameof(Conditioned)); }
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DF.Room2DEnergyPropertiesAbridged ToDragonfly()
        {
            return new DF.Room2DEnergyPropertiesAbridged(ConstructionSet.Name, ProgramType.Name, Hvac?.Name);
        }

        public HB.RoomEnergyPropertiesAbridged ToHoneybee()
        {
            throw new System.NotImplementedException();
        }
    }
}
