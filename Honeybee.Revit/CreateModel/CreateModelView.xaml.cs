using System.Linq;
using System.Windows.Controls;
using Honeybee.Revit.CreateModel.Wrappers;
using Honeybee.Revit.Schemas;

namespace Honeybee.Revit.CreateModel
{
    /// <summary>
    /// Interaction logic for CreateModelView.xaml
    /// </summary>
    public partial class CreateModelView
    {
        public CreateModelView()
        {
            InitializeComponent();
        }

        private CreateModelViewModel ViewModel
        {
            get { return DataContext as CreateModelViewModel; }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = ViewModel.SpatialObjects.Cast<SpatialObjectWrapper>().Where(so => so.IsSelected).ToList();
            if (e.AddedItems.Count > 0 && !(e.AddedItems[0] is NullAirSystem))
            {
                var hvac = e.AddedItems[0] as HvacTypes;
                foreach (var so in selected)
                {
                    so.Room2D.Properties.Energy.Conditioned = true;
                    so.Room2D.Properties.Energy.Hvac = hvac;
                }
            }
            else
            {
                foreach (var so in selected)
                {
                    so.Room2D.Properties.Energy.Conditioned = false;
                    so.Room2D.Properties.Energy.Hvac = null;
                }
            }
        }
    }
}
