using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using VisionClient.Core.Extends;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;

namespace VisionClient.Core
{
    public interface IStaticData
    {
        ObservableCollection<GameModel> GameModels { get; set; }
        UserDataModel UserData { get; set; }
        ObservableCollection<BaseUserModel> PendingFriendsList { get; set; }
        ObservableCollection<BaseUserModel> FriendRequestsList { get; set; }
        ExtendedObservableCollection<UserModel> FriendsList { get; set; }
        ObservableCollection<SearchModel> FoundUsersList { get; set; }
        ObservableCollection<MessageModel> Messages { get; set; }
        Guid SessionId { get; set; }
        Guid ChatId { get; set; }
        int MaxPages { get; set; }
        bool IsMainWindowVisible { get; set; }
        void ClearStatics();
    }

    public class StaticData : NotifyPropertyChanged ,IStaticData
    {
        public StaticData()
        {
            FriendsList.CollectionChanged += TestCollection;
        }
        private void TestCollection(object? sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(FriendsList));
        
        public ObservableCollection<GameModel> GameModels { get; set; } = new();
        public UserDataModel UserData { get; set; } = new();
        public ObservableCollection<BaseUserModel> PendingFriendsList { get; set; } = new();
        public ObservableCollection<BaseUserModel> FriendRequestsList { get; set; } = new();
        public ExtendedObservableCollection<UserModel> FriendsList { get; set; } = new();
        public ObservableCollection<SearchModel> FoundUsersList { get; set; } = new();
        public ObservableCollection<MessageModel> Messages { get; set; } = new();
        public Guid SessionId { get; set; }
        public Guid ChatId { get; set; }
        public int MaxPages { get; set; }
        public bool IsMainWindowVisible { get; set; }

        public void ClearStatics()
        {
            SessionId = Guid.Empty;
            ChatId = Guid.Empty;
            MaxPages = 1;
            UserData = new();
            FriendRequestsList.Clear();
            PendingFriendsList.Clear();
            FriendsList.Clear();
            FoundUsersList.Clear();
            GameModels.Clear();
            Messages.Clear();
        }
    }
}
