using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Windows;
using VisionClient.Core;
using VisionClient.Core.Enums;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.SignalR;

namespace VisionClient.ViewModels
{
    internal class SearchControlViewModel : BindableBase
    {
        private string username = string.Empty;
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }

        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set { SetProperty(ref loadingVisibility, value); }
        }

        public DelegateCommand<SearchModel> SendRequestCommand { get; }
        public DelegateCommand SearchUserCommand { get; }
        public IStaticData StaticData { get; }

        private readonly IUsersRepository usersRepository;
        private readonly IUsersService_Hubs usersService_Hubs;

        public SearchControlViewModel(IUsersRepository usersRepository, IUsersService_Hubs usersService_Hubs, IStaticData staticData)
        {
            SendRequestCommand = new DelegateCommand<SearchModel>(SendRequest);
            SearchUserCommand = new DelegateCommand(SearchUser);
            this.usersRepository = usersRepository;
            this.usersService_Hubs = usersService_Hubs;
            StaticData = staticData;
        }

        public async void SearchUser()
        {
            if (string.IsNullOrEmpty(Username)) return;

            try
            {
                StaticData.FoundUsersList.Clear();
                LoadingVisibility = Visibility.Visible;

                var list = await usersRepository.FindUsers(Username);
                foreach (var user in list)
                {
                    var isFriend = StaticData.FriendsList.Any(x => x.UserId == user.UserId);
                    var isPending = StaticData.PendingFriendsList.Any(x => x.UserId == user.UserId);
                    var isRequested = StaticData.FriendRequestsList.Any(x => x.UserId == user.UserId);

                    var searchModel = new SearchModel()
                    {
                        User = user,
                        IsRequestable = !isFriend && !isPending && !isRequested
                    };

                    StaticData.FoundUsersList.Add(searchModel);
                }

                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private async void SendRequest(SearchModel user)
        {
            try
            {
                await usersService_Hubs.Send(UserServiceHubs.Friends, "SendFriendRequest", new { Receiver = user.User.UserId });
            }
            catch (Exception) { }
        }
    }
}
