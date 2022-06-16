using System;
using System.Globalization;
using System.Windows.Data;
using VisionClient.Core.Enums;

namespace VisionClient.Converters
{
    public class StatusToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return 0.3;
            return (Status)value == Status.Offline || (Status)value == Status.Invisible ? 0.3 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return 0.3;
            return (Status)value == Status.Offline || (Status)value == Status.Invisible ? 0.3 : 1;
        }
    }
}
