using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisionClient.Converters
{
    public class IsBlockedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? "Unblock" : "Block";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? "Unblock" : "Block";
        }
    }
}
