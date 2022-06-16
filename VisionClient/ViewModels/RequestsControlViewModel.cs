using Prism.Commands;
using Prism.Mvvm;
using System;
using VisionClient.Core;
using VisionClient.Core.Enums;
using VisionClient.Core.Models;
using VisionClient.SignalR;

namespace VisionClient.ViewModels
{
    internal class RequestsControlViewModel : BindableBase
    {
        private readonly IUsersService_Hubs usersService_Hubs;

        public DelegateCommand<BaseUserModel> AcceptRequestCommand { get; }
        public DelegateCommand<BaseUserModel> DeclineRequestCommand { get; }
        public IStaticData StaticData { get; }

        public RequestsControlViewModel(IUsersService_Hubs usersService_Hubs, IStaticData staticData)
        {
            AcceptRequestCommand = new DelegateCommand<BaseUserModel>(AcceptRequest);
            DeclineRequestCommand = new DelegateCommand<BaseUserModel>(DeclineRequest);
            this.usersService_Hubs = usersService_Hubs;
            StaticData = staticData;
        }

        private async void AcceptRequest(BaseUserModel user)
        {
            try
            {
                await usersService_Hubs.Send(UserServiceHubs.Friends, "AcceptFriendRequest", user.UserId);
            }
            catch (Exception) { }
        }

        private async void DeclineRequest(BaseUserModel user)
        {
            try
            {
                await usersService_Hubs.Send(UserServiceHubs.Friends, "DeclineFriendRequest", user.UserId);
            }
            catch (Exception) { }
        }
    }
}
