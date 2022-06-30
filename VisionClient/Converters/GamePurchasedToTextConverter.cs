using System;
using System.Globalization;
using System.Windows.Data;

namespace VisionClient.Converters
{
    public class GamePurchasedToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return "Buy";
            bool isPurchased = (bool)value;
            return isPurchased ? "Play" : "Buy";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return "Buy";
            bool isPurchased = (bool)value;
            return isPurchased ? "Play" : "Buy";
        }
    }
}
