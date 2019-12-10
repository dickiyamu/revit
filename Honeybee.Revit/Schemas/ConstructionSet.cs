using System.ComponentModel;
using Honeybee.Revit.Schemas.Enumerations;

namespace Honeybee.Revit.Schemas
{
    public class ConstructionSet : INotifyPropertyChanged
    {
        private Vintages _vintage = Vintages.Vintage2013;
        public Vintages Vintage
        {
            get { return _vintage; }
            set { _vintage = value; RaisePropertyChanged(nameof(Vintage)); RaisePropertyChanged(nameof(Name)); }
        }

        private ClimateZones _climateZone = ClimateZones.Mixed;
        public ClimateZones ClimateZone
        {
            get { return _climateZone; }
            set { _climateZone = value; RaisePropertyChanged(nameof(ClimateZone)); RaisePropertyChanged(nameof(Name)); }
        }

        private ConstructionTypes _constructionType = ConstructionTypes.SteelFramed;
        public ConstructionTypes ConstructionType
        {
            get { return _constructionType; }
            set { _constructionType = value; RaisePropertyChanged(nameof(ConstructionType)); RaisePropertyChanged(nameof(Name)); }
        }

        public string Name
        {
            get { return $"{Vintage.DisplayName}::{ClimateZone.DisplayName}::{ConstructionType.DisplayName}"; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
