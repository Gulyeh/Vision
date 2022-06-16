using System;
using System.Globalization;
using System.Windows.Data;

namespace VisionClient.Converters
{
    internal class BoolNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return false;
            return (bool)value != true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return false;
            return (bool)value != true;
        }
    }
}
