using System.Windows.Controls;
using Honeybee.Revit.ModelSettings.Geometry.Wrappers;

namespace Honeybee.Revit.ModelSettings.Geometry
{
    /// <summary>
    /// Interaction logic for Geometry.xaml
    /// </summary>
    public partial class GeometryControl
    {
        public GeometryControl()
        {
            InitializeComponent();
        }

        private GeometryViewModel ViewModel
        {
            get { return DataContext as GeometryViewModel; }
        }

        private void GlazingTypesComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!((sender as ComboBox)?.SelectedItem is GlazingTypeWrapper ptw)) return;
            if (AppSettings.Instance.StoredSettings.GeometrySettings.GlazingTypes.Contains(ptw)) return;

            AppSettings.Instance.StoredSettings.GeometrySettings.GlazingTypes.Add(ptw);
            ViewModel.GlazingTypes.Remove(ptw);
        }
    }
}
