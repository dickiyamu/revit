using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Honeybee.Core.WPF.Buttons
{
    public class ImageToggleButton : ToggleButton
    {
        static ImageToggleButton()
        {
        }

        public ImageSource MainImage
        {
            get { return (ImageSource)GetValue(MainImageProperty); }
            set { SetValue(MainImageProperty, value); }
        }

        public static readonly DependencyProperty MainImageProperty =
            DependencyProperty.Register("MainImage", typeof(ImageSource), typeof(ImageToggleButton),
                new UIPropertyMetadata(null));

        public ImageSource HoverImage
        {
            get { return (ImageSource)GetValue(HoverImageProperty); }
            set { SetValue(HoverImageProperty, value); }
        }

        public static readonly DependencyProperty HoverImageProperty =
            DependencyProperty.Register("HoverImage", typeof(ImageSource), typeof(ImageToggleButton),
                new UIPropertyMetadata(null));

        public ImageSource DisabledImage
        {
            get { return (ImageSource)GetValue(DisabledImageProperty); }
            set { SetValue(DisabledImageProperty, value); }
        }

        public static readonly DependencyProperty DisabledImageProperty =
            DependencyProperty.Register("DisabledImage", typeof(ImageSource), typeof(ImageToggleButton),
                new UIPropertyMetadata(null));
    }
}
