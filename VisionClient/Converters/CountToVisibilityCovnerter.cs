using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VisionClient.Converters
{
    internal class CountToVisibilityCovnerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return Visibility.Collapsed;
            return (int)value == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return Visibility.Collapsed;
            return (int)value == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
