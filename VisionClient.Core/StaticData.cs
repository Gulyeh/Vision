using System.Collections.ObjectModel;
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
        ObservableCollection<UserModel> FriendsList { get; set; }
        ObservableCollection<SearchModel> FoundUsersList { get; set; }
        ObservableCollection<MessageModel> Messages { get; set; }
        Guid SessionId { get; set; }
        Guid ChatId { get; set; }
        int MaxPages { get; set; }
        void ClearStatics();
    }

    public class StaticData : NotifyPropertyChanged, IStaticData
    {
        public ObservableCollection<GameModel> GameModels { get; set; } = new();
        public UserDataModel UserData { get; set; } = new();
        public ObservableCollection<BaseUserModel> PendingFriendsList { get; set; } = new();
        public ObservableCollection<BaseUserModel> FriendRequestsList { get; set; } = new();
        public ObservableCollection<UserModel> FriendsList { get; set; } = new();
        public ObservableCollection<SearchModel> FoundUsersList { get; set; } = new();
        public ObservableCollection<MessageModel> Messages { get; set; } = new();
        public Guid SessionId { get; set; }
        public Guid ChatId { get; set; }
        public int MaxPages { get; set; }

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
