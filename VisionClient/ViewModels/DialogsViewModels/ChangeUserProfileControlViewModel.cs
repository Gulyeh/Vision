using Prism.Events;
using Prism.Services.Dialogs;
using System;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Helpers;
using VisionClient.SignalR;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class ChangeUserProfileControlViewModel : DialogHelper
    {
        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private readonly IUsersService_Hubs usersService_Hubs;
        private readonly IStaticData StaticData;

        public ChangeUserProfileControlViewModel(IEventAggregator eventAggregator, IUsersService_Hubs usersService_Hubs, IStaticData staticData) : base(eventAggregator)
        {
            this.usersService_Hubs = usersService_Hubs;
            this.StaticData = staticData;
        }

        protected override async void Execute(object? data)
        {
            try
            {
                ErrorText = string.Empty;

                var userData = new ChangedUserDataDto();
                if (string.IsNullOrEmpty(Content) || string.IsNullOrWhiteSpace(Content)) return;

                userData.Description = Title.Contains("Description") ? Content : StaticData.UserData.Description;
                userData.Username = Title.Contains("Username") ? Content : StaticData.UserData.Username;
                await usersService_Hubs.Send(UserServiceHubs.Users, "ChangeUserData", userData);
                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }
            catch (Exception e)
            {
                ErrorText = "Something went wrong";
            }
        }
    }
}
