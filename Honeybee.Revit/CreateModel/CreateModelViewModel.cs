using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Honeybee.Core.WPF;

namespace Honeybee.Revit.CreateModel
{
    public class CreateModelViewModel : ViewModelBase
    {
        public CreateModelModel Model { get; set; }

        public RelayCommand<Window> Cancel { get; set; }
        public RelayCommand<Window> Ok { get; set; }
        public RelayCommand Help { get; set; }
        public RelayCommand<Window> WindowLoaded { get; set; }

        public CreateModelViewModel(CreateModelModel model)
        {
            Model = model;

            Cancel = new RelayCommand<Window>(OnCancel);
            Ok = new RelayCommand<Window>(OnOk);
            Help = new RelayCommand(OnHelp);
            WindowLoaded = new RelayCommand<Window>(OnWindowLoaded);
        }

        private static void OnCancel(Window win)
        {
            win.Close();
        }

        private void OnOk(Window win)
        {
            win.Close();
        }

        private static void OnHelp()
        {
            Process.Start("");
        }

        private void OnWindowLoaded(Window win)
        {
            StatusBarManager.ProgressBar = ((CreateModelView)win).ProgressBar;
            StatusBarManager.StatusLabel = ((CreateModelView)win).StatusLabel;
        }
    }
}
