﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VisionClient.Converters
{
    internal class NullToVisibility : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] == DependencyProperty.UnsetValue || value[1] == DependencyProperty.UnsetValue || value[2] == DependencyProperty.UnsetValue) return Visibility.Collapsed;
            return ((Guid)value[0] == (Guid)value[2]) && (DateTime?)value[1] != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
