using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using VisionClient.Core.Enums;
using VisionClient.Core.Models;

namespace VisionClient.Converters.FriendsConverters
{
    internal class GetOfflineFriendsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var friends = (ObservableCollection<UserModel>)value;
            if (friends is null) return new ObservableCollection<UserModel>();
            var offlineFriends = friends.Where(x => x.Status != Status.Online && x.Status != Status.Away);
            return offlineFriends.OrderBy(x => x.Username);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
