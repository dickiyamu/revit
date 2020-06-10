using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Honeybee.Core.WPF.Buttons;

namespace Honeybee.Core.WPF
{
    /// <summary>
    /// Class used to initialize and step through the progress bar.
    /// </summary>
    public static class StatusBarManager
    {
        public static ProgressBar ProgressBar = null;
        public static TextBlock StatusLabel = null;
        public static ImageButton LogButton = null;
        public static double ProgressValue;
        public static List<string> Logs = null;

        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
        private delegate void UpdateStatusLabelDelegate(DependencyProperty dp, object value);

        private static UpdateStatusLabelDelegate _updateLabelDelegate;
        private static UpdateProgressBarDelegate _updatePbDelegate;

        public static void SetStatus(string message)
        {
            if (StatusLabel == null)
                return;

            if (_updateLabelDelegate == null)
                _updateLabelDelegate = StatusLabel.SetValue;

            Dispatcher.CurrentDispatcher.Invoke(_updateLabelDelegate, DispatcherPriority.Background, TextBlock.TextProperty, message);
        }

        public static void InitializeProgressIndeterminate(string statusText)
        {
            if (null == ProgressBar || null == StatusLabel) return;

            ProgressBar.Visibility = Visibility.Visible;

            _updateLabelDelegate = StatusLabel.SetValue;
            Dispatcher.CurrentDispatcher.Invoke(_updateLabelDelegate, DispatcherPriority.Background, TextBlock.TextProperty, statusText);

            _updatePbDelegate = ProgressBar.SetValue;
            ProgressBar.IsIndeterminate = true;

            Logs = new List<string>();
            LogButton.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Initialize Progress Bar and set Status Label.
        /// </summary>
        /// <param name="statusText">Status Label initial value.</param>
        /// <param name="maximum">Max count for the Progress Bar.</param>
        public static void InitializeProgress(string statusText, int maximum)
        {
            if (null == ProgressBar || null == StatusLabel) return;

            ProgressBar.Visibility = Visibility.Visible;
            ProgressValue = 0;

            _updateLabelDelegate = StatusLabel.SetValue;
            Dispatcher.CurrentDispatcher.Invoke(_updateLabelDelegate, DispatcherPriority.Background, TextBlock.TextProperty, statusText);

            _updatePbDelegate = ProgressBar.SetValue;
            ProgressBar.Value = ProgressValue;
            ProgressBar.Maximum = maximum;
        }

        /// <summary>
        /// Iterate Progress Bar by one.
        /// </summary>
        public static void StepForward()
        {
            if (null == ProgressBar || null == StatusLabel) return;

            ProgressValue++;
            Dispatcher.CurrentDispatcher.Invoke(_updatePbDelegate, DispatcherPriority.Background, RangeBase.ValueProperty, ProgressValue);
        }

        /// <summary>
        /// Iterate Progress Bar by one.
        /// </summary>
        public static void StepForward(string status)
        {
            if (null == ProgressBar || null == StatusLabel) return;

            ProgressValue++;
            Dispatcher.CurrentDispatcher.Invoke(_updatePbDelegate, DispatcherPriority.Background, RangeBase.ValueProperty, ProgressValue);
            Dispatcher.CurrentDispatcher.Invoke(_updateLabelDelegate, DispatcherPriority.Background, TextBlock.TextProperty, status);
        }

        /// <summary>
        /// Clean up Progress bar by resetting Status Label.
        /// </summary>
        public static void FinalizeProgress()
        {
            if (null == ProgressBar || null == StatusLabel) return;

            ProgressValue = 0;
            ProgressBar.Visibility = Visibility.Hidden;
            StatusLabel.Text = "Ready.";
        }

        public static void FinalizeProgressIndeterminate()
        {
            if (null == ProgressBar || null == StatusLabel) return;

            ProgressValue = 0;
            ProgressBar.Visibility = Visibility.Hidden;
            ProgressBar.IsIndeterminate = false;
            StatusLabel.Text = "Ready.";
        }
    }
}
