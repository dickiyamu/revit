using System.Windows.Forms;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Honeybee.Core.WPF
{
    public static class Dialogs
    {
        public static string SelectFile(string filter = "", string ext = "", string title = "Select File")
        {
            var dialog = new OpenFileDialog
            {
                Filter = filter,
                DefaultExt = ext,
                Title = title
            };

            return dialog.ShowDialog() == true ? dialog.FileName : string.Empty;
        }

        public static string SelectDirectory()
        {
            var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };
            var result = dialog.ShowDialog();

            return result != DialogResult.OK ? string.Empty : dialog.SelectedPath;
        }
    }
}
