using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VisionClient.Converters
{
    public class HorizontalChatConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] == DependencyProperty.UnsetValue || value[1] == DependencyProperty.UnsetValue) return HorizontalAlignment.Right;
            return (Guid)value[0] != (Guid)value[1] ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
