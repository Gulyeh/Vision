using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using VisionClient.Core.Enums;

namespace VisionClient.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                Status.Online => new SolidColorBrush(Colors.Green),
                Status.Invisible => new SolidColorBrush(Colors.Gray),
                Status.Away => new SolidColorBrush(Colors.Yellow),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                Status.Online => new SolidColorBrush(Colors.Green),
                Status.Invisible => new SolidColorBrush(Colors.Gray),
                Status.Away => new SolidColorBrush(Colors.Yellow),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
    }
}
