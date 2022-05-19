using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VisionClient.Core.Enums;

namespace VisionClient.Converters
{
    public class StatusToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Status)value == Status.Offline || (Status)value == Status.Invisible ? 0.3 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Status)value == Status.Offline || (Status)value == Status.Invisible ? 0.3 : 1;
        }
    }
}
