using System.ComponentModel;
using Honeybee.Revit.Schemas.Converters;
using Honeybee.Revit.Schemas.Enumerations;
using Newtonsoft.Json;
using HB = HoneybeeSchema;
// ReSharper disable NotAccessedField.Local

namespace Honeybee.Revit.Schemas
{
    [JsonConverter(typeof(ConstructionSetConverter))]
    public class ConstructionSet : IBaseObject, INotifyPropertyChanged
    {
        [JsonProperty("type")]
        public string Type
        {
            get { return GetType().Name; }
        }

        private string _identifier;
        [JsonProperty("identifier")]
        public string Identifier
        {
            get { return $"{Vintage.DisplayName}::{ClimateZone.DisplayName}::{ConstructionType.DisplayName}"; }
            set { _identifier = value; RaisePropertyChanged(nameof(Identifier)); }
        }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("wall_set")]
        public HB.WallConstructionSet WallSet { get; set; }

        [JsonProperty("floor_set")]
        public HB.FloorConstructionSet FloorSet { get; set; }

        [JsonProperty("roof_ceiling_set")]
        public HB.RoofCeilingConstructionSet RoofCeilingSet { get; set; }

        [JsonProperty("aperture_set")]
        public HB.ApertureConstructionSet ApertureSet { get; set; }

        [JsonProperty("door_set")]
        public HB.DoorConstructionSet DoorSet { get; set; }

        [JsonProperty("shade_construction")]
        public HB.ShadeConstruction ShadeConstruction { get; set; }

        [JsonProperty("air_boundary_construction")]
        public HB.AirBoundaryConstruction AirBoundaryConstruction { get; set; }

        private Vintages _vintage = Vintages.Vintage2013;
        [JsonIgnore]
        public Vintages Vintage
        {
            get { return _vintage; }
            set { _vintage = value; RaisePropertyChanged(nameof(Vintage)); RaisePropertyChanged(nameof(Identifier)); }
        }

        private ClimateZones _climateZone = ClimateZones.Mixed;
        [JsonIgnore]
        public ClimateZones ClimateZone
        {
            get { return _climateZone; }
            set { _climateZone = value; RaisePropertyChanged(nameof(ClimateZone)); RaisePropertyChanged(nameof(Identifier)); }
        }

        private ConstructionTypes _constructionType = ConstructionTypes.SteelFramed;
        [JsonIgnore]
        public ConstructionTypes ConstructionType
        {
            get { return _constructionType; }
            set { _constructionType = value; RaisePropertyChanged(nameof(ConstructionType)); RaisePropertyChanged(nameof(Identifier)); }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
