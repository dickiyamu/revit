using System.Windows;
using System.Windows.Input;

namespace Honeybee.Revit.ModelSettings.Geometry.Controls
{
    public sealed partial class GlazingControl
    {
        public GlazingControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.Register("ButtonCommand",
            typeof(ICommand), typeof(GlazingControl), new UIPropertyMetadata(null));

        public ICommand ButtonCommand
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }
    }
}
