using System;
using System.Globalization;
using System.Windows.Data;

namespace VisionClient.Converters
{
    public class BooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return 0.5;

            var isPurchased = (bool)value;
            return isPurchased ? 1 : 0.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return 0.5;

            var isPurchased = (bool)value;
            return isPurchased ? 1 : 0.5;
        }
    }
}
