using Prism.Commands;
using Prism.Mvvm;
using System;
using VisionClient.Core;
using VisionClient.Core.Enums;
using VisionClient.Core.Models;
using VisionClient.SignalR;

namespace VisionClient.ViewModels
{
    internal class PendingControlViewModel : BindableBase
    {
        private readonly IUsersService_Hubs usersService_Hubs;

        public DelegateCommand<BaseUserModel> CancelPendingCommand { get; }
        public IStaticData StaticData { get; }

        public PendingControlViewModel(IUsersService_Hubs usersService_Hubs, IStaticData staticData)
        {
            CancelPendingCommand = new DelegateCommand<BaseUserModel>(CancelPending);
            this.usersService_Hubs = usersService_Hubs;
            StaticData = staticData;
        }

        private async void CancelPending(BaseUserModel user)
        {
            try
            {
                await usersService_Hubs.Send(UserServiceHubs.Friends, "DeclineFriendRequest", user.UserId);
            }
            catch (Exception) { }
        }
    }
}
