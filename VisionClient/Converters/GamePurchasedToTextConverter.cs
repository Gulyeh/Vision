using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisionClient.Converters
{
    public class GamePurchasedToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isPurchased = (bool)value;
            return isPurchased ? "Play" : "Buy";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isPurchased = (bool)value;
            return isPurchased ? "Play" : "Buy";
        }
    }
}
