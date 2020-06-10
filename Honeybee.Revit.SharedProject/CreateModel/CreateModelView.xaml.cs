using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Honeybee.Core.WPF;
using Honeybee.Revit.CreateModel.Wrappers;
using Honeybee.Revit.Schemas;
using NLog.Fluent;

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

        //private void btnRightMenuHide_Click(object sender, RoutedEventArgs e)
        //{
        //    ShowHideMenu("sbHideRightMenu", btnRightMenuHide, btnRightMenuShow, pnlRightMenu);
        //}

        //private void btnRightMenuShow_Click(object sender, RoutedEventArgs e)
        //{
        //    ShowHideMenu("sbShowRightMenu", btnRightMenuHide, btnRightMenuShow, pnlRightMenu);
        //}

        //private void ShowHideMenu(string Storyboard, Button btnHide, Button btnShow, StackPanel pnl)
        //{
        //    Storyboard sb = Resources[Storyboard] as Storyboard;
        //    sb.Begin(pnl);

        //    if (Storyboard.Contains("Show"))
        //    {
        //        btnHide.Visibility = System.Windows.Visibility.Visible;
        //        btnShow.Visibility = System.Windows.Visibility.Hidden;
        //    }
        //    else if (Storyboard.Contains("Hide"))
        //    {
        //        btnHide.Visibility = System.Windows.Visibility.Hidden;
        //        btnShow.Visibility = System.Windows.Visibility.Visible;
        //    }
        //}

        private void CreateModelView_OnLoaded(object sender, RoutedEventArgs e)
        {
            StatusBarManager.ProgressBar = ProgressBar;
            StatusBarManager.StatusLabel = StatusLabel;
            StatusBarManager.LogButton = LogButton;

            var vm = DataContext as CreateModelViewModel;
            vm.OnWindowLoaded();
        }
    }
}
