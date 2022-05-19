using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VisionClient.Converters
{
    public class NegationHorizontalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (HorizontalAlignment)value == HorizontalAlignment.Left ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (HorizontalAlignment)value == HorizontalAlignment.Left ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }
    }
}
