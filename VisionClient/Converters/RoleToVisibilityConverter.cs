using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VisionClient.Converters
{
    internal class RoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || parameter is null) return Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(value.ToString())) return Visibility.Collapsed;

            switch ((string)parameter)
            {
                case "AdminPanelButton":
                    return (string)value == "User" ? Visibility.Collapsed : Visibility.Visible;
                case "AdminPanelSettings":
                    return (string)value == "Moderator" || (string)value == "User" ? Visibility.Collapsed : Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
