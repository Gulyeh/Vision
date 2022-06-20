using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VisionClient.Converters
{
    internal class NegationCountToVisibilityCovnerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return Visibility.Visible;
            return (int)value == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
