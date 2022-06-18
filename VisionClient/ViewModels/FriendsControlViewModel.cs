using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using VisionClient.Core;
using VisionClient.Core.Enums;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.SignalR;

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

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IDialogService dialogService;
        private readonly IUsersService_Hubs usersService_Hubs;

        public IStaticData StaticData { get; }
        public DelegateCommand<string> ChangeStatusCommand { get; }
        public DelegateCommand<UserModel> BlockFriendCommand { get; }
        public DelegateCommand<UserModel> DeleteFriendCommand { get; }
        public DelegateCommand<string> OpenControlCommand { get; }

        public FriendsControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IDialogService dialogService,
            IUsersService_Hubs usersService_Hubs, IStaticData staticData)
        {
            ChangeStatusCommand = new DelegateCommand<string>(ChangeUserStatus);
            BlockFriendCommand = new DelegateCommand<UserModel>(BlockUser);
            DeleteFriendCommand = new DelegateCommand<UserModel>(DeleteUser);
            OpenControlCommand = new DelegateCommand<string>(OpenControl);

            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                SelectedUser = null;
            }, ThreadOption.PublisherThread, false, x => x == "StopFocus");

            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.usersService_Hubs = usersService_Hubs;
            this.StaticData = staticData;
        }

        private void OpenControl(string name) => regionManager.RequestNavigate("LibraryContentRegion", name);

        private async void BlockUser(UserModel user)
        {
            try
            {
                if (user == null) return;
                var userBlocked = user.IsBlocked ? "Unblock" : "Block";
                var userName = user.Username;
                var message = $"Do you want to {userBlocked} {userName}?";
                var title = $"{userBlocked} {userName}";
                if (ShowDialog(message, title)) await usersService_Hubs.Send(UserServiceHubs.Friends, "ToggleBlockUser", user.UserId);
            }
            catch (Exception) { }
        }

        private async void DeleteUser(UserModel user)
        {
            try
            {
                if (user == null) return;
                var userName = user.Username;
                var message = $"Do you want to delete {userName} from friends list?";
                var title = $"Delete {userName}";
                if (ShowDialog(message, title))
                {
                    eventAggregator.GetEvent<SendEvent<Guid>>().Publish(user.UserId);
                    await usersService_Hubs.Send(UserServiceHubs.Friends, "DeleteFriend", user.UserId);
                }
            }
            catch (Exception) { }
        }

        private bool ShowDialog(string message, string title)
        {
            var dialogParams = new DialogParameters
            {
                { "message", message },
                { "title", title }
            };

            bool result = false;

            dialogService.ShowDialog("ConfirmControl", dialogParams, r =>
            {
                result = r.Result switch
                {
                    ButtonResult.OK => true,
                    ButtonResult.Cancel => false,
                    _ => false,
                };
            });

            return result;
        }

        private async void ChangeUserStatus(string status)
        {
            var parsed = Enum.TryParse(status, out Status enumStatus);
            if (!parsed || enumStatus == StaticData.UserData.Status) return;

            await usersService_Hubs.Send(UserServiceHubs.Users, "ChangeUserStatus", enumStatus);
        }

        private void ShowMessage(UserModel user)
        {
            regionManager.RequestNavigate("LibraryContentRegion", "MessageControl");
            eventAggregator.GetEvent<SendEvent<UserModel>>().Publish(user);
        }
    }
}
