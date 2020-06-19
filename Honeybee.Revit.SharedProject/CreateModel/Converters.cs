using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Honeybee.Core;
using Honeybee.Revit.CreateModel.Wrappers;

namespace Honeybee.Revit.CreateModel
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value != null && (bool)value;
            return v ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value != null && (bool)value;
            return v ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LevelToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as LevelWrapper)?.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumToCollectionConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null
                ? EnumUtils.GetAllValuesAndDescriptions(value.GetType())
                : new List<ValueDescription>();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class FilterTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            var filterType = (FilterType)value;
            switch (filterType)
            {
                case FilterType.None:
                    return Visibility.Collapsed;
                case FilterType.Level:
                    return Visibility.Visible;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessagesToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is List<string> messages)) return Visibility.Visible;

            return messages.Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValidateRunButtonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var epwPath = (string) values[0];
            var simDir = (string) values[1];

            return !string.IsNullOrWhiteSpace(epwPath) && !string.IsNullOrWhiteSpace(simDir);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ListToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException("The target must be a String");

            return string.Join(", ", ((List<string>)value)?.ToArray() ?? throw new InvalidOperationException());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //public class StyleConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is bool dragonfly && dragonfly)
    //            return "ContentDataGridCenteringDf";

    //        return "ContentDataGridCenteringHb";
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class StyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var targetElement = (FrameworkElement)values[0];
            var dragonfly = (bool)values[1];

            if (targetElement == null)
                return null;

            var resourceName = dragonfly ? "ContentDataGridCenteringDf" : "ContentDataGridCenteringHb";
            var newStyle = (Style)targetElement.TryFindResource(resourceName);

            return newStyle;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ResourceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var dragonfly = (bool)values[0];
            var resourceName = (string) values[1];

            var dfString = dragonfly ? "Df" : "";
            switch (resourceName)
            {
                case "icon":
                    var iconString = dragonfly ? "dragonfly" : "honeybee";
                    return new BitmapImage(new Uri($"pack://application:,,,/Honeybee.Core;Component/Resources/{iconString}.ico"));
                case "pick":
                    return new BitmapImage(new Uri($"pack://application:,,,/Honeybee.Core;Component/Resources/24x24/pick{dfString}.png"));
                case "delete":
                    return new BitmapImage(new Uri($"pack://application:,,,/Honeybee.Core;Component/Resources/24x24/delete{dfString}.png"));
                case "export":
                    return new BitmapImage(new Uri($"pack://application:,,,/Honeybee.Core;Component/Resources/24x24/export{dfString}.png"));
                default:
                    return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
