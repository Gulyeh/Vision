using System;
using System.Globalization;
using System.Windows.Data;

namespace VisionClient.Converters
{
    public class IsBlockedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return "Block";
            return (bool)value == true ? "Unblock" : "Block";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return "Block";
            return (bool)value == true ? "Unblock" : "Block";
        }
    }
}
