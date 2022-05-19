using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using VisionClient.Core;

namespace VisionClient.Converters
{
    public class UserIdToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value != StaticData.UserData.Id ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value != StaticData.UserData.Id ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
