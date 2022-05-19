using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisionClient.Converters
{
    public class BooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isPurchased = (bool)value;
            return isPurchased ? 1 : 0.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isPurchased = (bool)value;
            return isPurchased ? 1 : 0.5;
        }
    }
}
