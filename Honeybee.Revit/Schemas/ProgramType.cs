using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Honeybee.Core;
using Honeybee.Core.Extensions;
using Honeybee.Revit.Schemas.Enumerations;

namespace Honeybee.Revit.Schemas
{
    public class ProgramType : INotifyPropertyChanged
    {
        private Vintages _vintage = Vintages.Vintage2013;
        public Vintages Vintage
        {
            get { return _vintage; }
            set
            {
                _vintage = value;
                RaisePropertyChanged(nameof(Vintage));
                RaisePropertyChanged(nameof(BuildingProgramsValues));
                RaisePropertyChanged(nameof(RoomProgramsValues));
                RaisePropertyChanged(nameof(Name));
            }
        }

        private BuildingPrograms _buildingProgram = BuildingPrograms.MediumOffice;
        public BuildingPrograms BuildingProgram
        {
            get { return _buildingProgram; }
            set
            {
                _buildingProgram = value;
                RaisePropertyChanged(nameof(BuildingProgram));
                RaisePropertyChanged(nameof(RoomProgramsValues));
                RaisePropertyChanged(nameof(Name));
            }
        }

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

        private RoomPrograms _roomProgram = RoomPrograms.ClosedOffice;
        public RoomPrograms RoomProgram
        {
            get { return _roomProgram; }
            set { _roomProgram = value; RaisePropertyChanged(nameof(RoomProgram)); RaisePropertyChanged(nameof(Name)); }
        }

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

        public string Name
        {
            get { return $"{Vintage.DisplayName}::{BuildingProgram.DisplayName}::{RoomProgram.DisplayName}"; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
