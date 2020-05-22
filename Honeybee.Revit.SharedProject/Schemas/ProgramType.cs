using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Honeybee.Core;
using Honeybee.Core.Extensions;
using Honeybee.Revit.Schemas.Converters;
using Honeybee.Revit.Schemas.Enumerations;
using Newtonsoft.Json;
using HB = HoneybeeSchema;
// ReSharper disable NotAccessedField.Local

namespace Honeybee.Revit.Schemas
{
    [JsonConverter(typeof(ProgramTypeConverter))]
    public class ProgramType : IBaseObject, INotifyPropertyChanged
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
            get { return $"{Vintage.DisplayName}::{BuildingProgram.DisplayName}::{RoomProgram.DisplayName}"; }
            set { _identifier = value; RaisePropertyChanged(nameof(Identifier)); }
        }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("people")]
        public HB.People People { get; set; }

        [JsonProperty("lighting")]
        public HB.Lighting Lighting { get; set; }

        [JsonProperty("electric_equipment")]
        public HB.ElectricEquipment ElectricEquipment { get; set; }

        [JsonProperty("gas_equipment")]
        public HB.GasEquipment GasEquipment { get; set; }

        [JsonProperty("infiltration")]
        public HB.Infiltration Infiltration { get; set; }

        [JsonProperty("ventilation")]
        public HB.Ventilation Ventilation { get; set; }

        [JsonProperty("setpoint")]
        public HB.Setpoint SetPoint { get; set; }

        private Vintages _vintage = Vintages.Vintage2013;
        [JsonIgnore]
        public Vintages Vintage
        {
            get { return _vintage; }
            set
            {
                _vintage = value;
                RaisePropertyChanged(nameof(Vintage));
                RaisePropertyChanged(nameof(BuildingProgramsValues));
                RaisePropertyChanged(nameof(RoomProgramsValues));
                RaisePropertyChanged(nameof(Identifier));
            }
        }

        private BuildingPrograms _buildingProgram = BuildingPrograms.MediumOffice;
        [JsonIgnore]
        public BuildingPrograms BuildingProgram
        {
            get { return _buildingProgram; }
            set
            {
                _buildingProgram = value;
                RaisePropertyChanged(nameof(BuildingProgram));
                RaisePropertyChanged(nameof(RoomProgramsValues));
                RaisePropertyChanged(nameof(Identifier));
            }
        }

        private RoomPrograms _roomProgram = RoomPrograms.ClosedOffice;
        [JsonIgnore]
        public RoomPrograms RoomProgram
        {
            get { return _roomProgram; }
            set { _roomProgram = value; RaisePropertyChanged(nameof(RoomProgram)); RaisePropertyChanged(nameof(Identifier)); }
        }

        [JsonIgnore]
        public ObservableCollection<BuildingPrograms> BuildingProgramsValues
        {
            get
            {
                if (Equals(Vintage, Vintages.Vintage2013))
                {
                    return AppSettings.Instance.Rooms2013.Keys.Select(Enumeration.FromDisplayName<BuildingPrograms>)
                        .ToObservableCollection();
                }
                if (Equals(Vintage, Vintages.Vintage2010))
                {
                    return AppSettings.Instance.Rooms2010.Keys.Select(Enumeration.FromDisplayName<BuildingPrograms>)
                        .ToObservableCollection();
                }
                if (Equals(Vintage, Vintages.Vintage2007))
                {
                    return AppSettings.Instance.Rooms2007.Keys.Select(Enumeration.FromDisplayName<BuildingPrograms>)
                        .ToObservableCollection();
                }
                if (Equals(Vintage, Vintages.Vintage2004))
                {
                    return AppSettings.Instance.Rooms2004.Keys.Select(Enumeration.FromDisplayName<BuildingPrograms>)
                        .ToObservableCollection();
                }
                if (Equals(Vintage, Vintages.Vintage1980To2004))
                {
                    return AppSettings.Instance.Rooms1980To2004.Keys
                        .Select(Enumeration.FromDisplayName<BuildingPrograms>).ToObservableCollection();
                }

                return AppSettings.Instance.RoomsPre1980.Keys.Select(Enumeration.FromDisplayName<BuildingPrograms>)
                    .ToObservableCollection();
            }
        }

        [JsonIgnore]
        public ObservableCollection<RoomPrograms> RoomProgramsValues
        {
            get
            {
                if (Equals(Vintage, Vintages.Vintage2013))
                {
                    return AppSettings.Instance.Rooms2013[BuildingProgram.DisplayName]
                        .Select(Enumeration.FromDisplayName<RoomPrograms>).ToObservableCollection();
                }
                if (Equals(Vintage, Vintages.Vintage2010))
                {
                    return AppSettings.Instance.Rooms2010[BuildingProgram.DisplayName]
                        .Select(Enumeration.FromDisplayName<RoomPrograms>).ToObservableCollection();
                }
                if (Equals(Vintage, Vintages.Vintage2007))
                {
                    return AppSettings.Instance.Rooms2007[BuildingProgram.DisplayName]
                        .Select(Enumeration.FromDisplayName<RoomPrograms>).ToObservableCollection();
                }
                if (Equals(Vintage, Vintages.Vintage2004))
                {
                    return AppSettings.Instance.Rooms2004[BuildingProgram.DisplayName]
                        .Select(Enumeration.FromDisplayName<RoomPrograms>).ToObservableCollection();
                }
                if (Equals(Vintage, Vintages.Vintage1980To2004))
                {
                    return AppSettings.Instance.Rooms1980To2004[BuildingProgram.DisplayName]
                        .Select(Enumeration.FromDisplayName<RoomPrograms>).ToObservableCollection();
                }

                return AppSettings.Instance.RoomsPre1980[BuildingProgram.DisplayName]
                    .Select(Enumeration.FromDisplayName<RoomPrograms>).ToObservableCollection();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
