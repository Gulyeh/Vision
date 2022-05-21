using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core;
using VisionClient.Core.Enums;
using VisionClient.Core.Events;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels
{
    internal class FriendsControlViewModel : BindableBase
    {
        private UserModel? selectedUser;
        public UserModel? SelectedUser
        {
            get { return selectedUser; }
            set 
            { 
                SetProperty(ref selectedUser, value);
                if (SelectedUser is not null) ShowMessage(SelectedUser);
            }
        }

        private UserModel user = new();
        public UserModel User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }

        private int onlineCount = 0;
        public int OnlineCount
        {
            get { return onlineCount; }
            set { SetProperty(ref onlineCount, value); }
        }

        private int offlineCount = 0;
        public int OfflineCount
        {
            get { return offlineCount; }
            set { SetProperty(ref offlineCount, value); }
        }

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IDialogService dialogService;

        public ObservableCollection<UserModel> FriendsListOnline { get; set; }
        public ObservableCollection<UserModel> FriendsListOffline { get; set; }
        public DelegateCommand<string> ChangeStatusCommand { get; set; }
        public DelegateCommand<UserModel> BlockFriendCommand { get; set; }
        public DelegateCommand<UserModel> DeleteFriendCommand { get; set; }
        public DelegateCommand<string> OpenControlCommand { get; set; }

        public FriendsControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IDialogService dialogService)
        {
            User = StaticData.UserData;
            FriendsListOnline = new ObservableCollection<UserModel>();
            FriendsListOffline = new ObservableCollection<UserModel>();
            ChangeStatusCommand = new DelegateCommand<string>(ChangeUserStatus);
            BlockFriendCommand = new DelegateCommand<UserModel>(BlockUser);
            DeleteFriendCommand = new DelegateCommand<UserModel>(DeleteUser);
            OpenControlCommand = new DelegateCommand<string>(OpenControl);

            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                SelectedUser = null;
            }, ThreadOption.PublisherThread, false, x => x == "StopFocus");


            List<UserModel> FriendsList = new List<UserModel>();

            //TEST
            var user = new UserModel()
            {
                Id = new Guid(),
                UserName = "TestUserasdasdasdasdasdadasdadadasd",
                Description = "Test Descriptionasdasdadadsadadadasdada",
                IsBlocked = false,
                PhotoUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460__340.png",
                Status = Status.Online
            };
            var user1 = new UserModel()
            {
                Id = new Guid(),
                UserName = "TestUser1",
                IsBlocked = true,
                PhotoUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460__340.png",
                Status = Status.Offline
            };

            var user3 = new UserModel()
            {
                Id = new Guid(),
                UserName = "TestUserasdasdasdasdasdadasdadadasd",
                IsBlocked = false,
                PhotoUrl = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460__340.png",
                Status = Status.Away
            };


            FriendsList.Add(user1);
            FriendsList.Add(user);
            FriendsList.Add(user1);
            FriendsList.Add(user);
            FriendsList.Add(user1);
            FriendsList.Add(user);
            FriendsList.Add(user3);
            FriendsList.Add(user1);
            FriendsList.Add(user);
            FriendsList.Add(user1);
            FriendsList.Add(user);
            FriendsList.Add(user1);
            FriendsList.Add(user);

            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            FriendsListOnline = new ObservableCollection<UserModel>(FriendsList.Where(x => x.Status != Status.Invisible && x.Status != Status.Offline)
                .OrderBy(x => x.UserName));
            FriendsListOffline = new ObservableCollection<UserModel>(FriendsList.Where(x => x.Status != Status.Online && x.Status != Status.Away)
                .OrderBy(x => x.UserName));
            OnlineCount = FriendsListOnline.Count();
            OfflineCount = FriendsListOffline.Count();
        }

        private void OpenControl(string name)
        {
            regionManager.RequestNavigate("LibraryContentRegion", name);
        }

        private void BlockUser(UserModel user)
        {
            if (user == null) return;
            var userBlocked = user.IsBlocked ? "Unblock" : "Block";
            var userName = user.UserName;
            var message = $"Do you want to {userBlocked} {userName}?";
            var title = $"{userBlocked} {userName}";
            ShowDialog(message, title);
        }

        private void DeleteUser(UserModel user)
        {
            if (user == null) return;
            var userName = user.UserName;
            var message = $"Do you want to delete {userName} from friends list?";
            var title = $"Delete {userName}";
            ShowDialog(message, title);
        }

        private void ShowDialog(string message, string title)
        {
            var dialogParams = new DialogParameters
            {
                { "message", message },
                { "title", title }
            };

            dialogService.ShowDialog("ConfirmControl", dialogParams, null);
        }

        private void ChangeUserStatus(string status)
        {
            Enum.TryParse(status, out Status enumStatus);
            if(enumStatus == User.Status) return;
            User.Status = enumStatus;
            RaisePropertyChanged(nameof(User));
        }

        private void ShowMessage(UserModel user)
        {
            regionManager.RequestNavigate("LibraryContentRegion", "MessageControl");
            eventAggregator.GetEvent<SendEvent<UserModel>>().Publish(user);
        }
    }
}
