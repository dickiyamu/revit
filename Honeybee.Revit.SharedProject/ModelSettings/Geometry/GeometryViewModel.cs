using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Honeybee.Core.Extensions;
using Honeybee.Revit.ModelSettings.Geometry.Wrappers;

namespace Honeybee.Revit.ModelSettings.Geometry
{
    public class GeometryViewModel : ViewModelBase
    {
        public GeometryModel Model { get; set; }
        public RelayCommand<PanelTypeWrapper> RemoveGlazingType { get; set; }

        private AppSettings _settings;
        public AppSettings Settings
        {
            get { return _settings; }
            set { _settings = value; RaisePropertyChanged(() => Settings); }
        }

        private ObservableCollection<PanelTypeWrapper> _panelTypes;
        public ObservableCollection<PanelTypeWrapper> GlazingTypes
        {
            get { return _panelTypes; }
            set { _panelTypes = value; RaisePropertyChanged(() => GlazingTypes); }
        }

        public GeometryViewModel(GeometryModel model)
        {
            Model = model;
            Settings = AppSettings.Instance;

            GlazingTypes = Model.CollectPanels().ToObservableCollection();
            GlazingTypes.CollectionChanged += PanelTypesOnCollectionChanged;

            RemoveGlazingType = new RelayCommand<PanelTypeWrapper>(OnRemoveGlazingType);
        }

        private void OnRemoveGlazingType(PanelTypeWrapper ptw)
        {
            if (!Settings.StoredSettings.GeometrySettings.GlazingTypes.Contains(ptw)) return;

            Settings.StoredSettings.GeometrySettings.GlazingTypes.Remove(ptw);
            GlazingTypes.Add(ptw);
        }

        private void PanelTypesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => GlazingTypes);
        }
    }
}
