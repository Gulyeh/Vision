using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using VisionClient.Core.Enums;
using VisionClient.Core.Models;

namespace VisionClient.Converters.FriendsConverters
{
    internal class CountOnlineFriendsConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = (CollectionViewSource)parameter;
            var friends = (ObservableCollection<UserModel>)collection.Source;
            if (friends is null || friends.Count == 0) return "0";
            var onlineFriends = friends.Where(x => x.Status == Status.Online || x.Status == Status.Away);
            return onlineFriends is null ? "0" : onlineFriends.Count().ToString();
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
