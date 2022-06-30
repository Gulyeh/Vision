using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VisionClient.Converters
{
    public class NegationHorizontalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return HorizontalAlignment.Right;
            return (HorizontalAlignment)value == HorizontalAlignment.Left ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return HorizontalAlignment.Right;
            return (HorizontalAlignment)value == HorizontalAlignment.Left ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }
    }
}
