using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Enums;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Helpers;
using VisionClient.SignalR;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class KickControlViewModel : BindableBase
    {
        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private bool isButtonEnabled = true;
        public bool IsButtonEnabled
        {
            get { return isButtonEnabled; }
            set { SetProperty(ref isButtonEnabled, value); }
        }

        private KickUserDto kickModel = new();
        public KickUserDto KickModel
        {
            get { return kickModel; }
            set { SetProperty(ref kickModel, value); }
        }

        private readonly IUsersService_Hubs usersService_Hubs;
        private readonly IStaticData staticData;
        private readonly IDialogService dialogService;

        public DelegateCommand ExecuteCommand { get; }

        public KickControlViewModel(IUsersService_Hubs usersService_Hubs, IEventAggregator eventAggregator, IStaticData staticData, ITextEventHelper panelOpened,
                IDialogService dialogService)
        {
            ExecuteCommand = new DelegateCommand(Execute);
            this.usersService_Hubs = usersService_Hubs;
            this.staticData = staticData;
            this.dialogService = dialogService;

            eventAggregator.GetEvent<SendEvent<DetailedUserModel>>().Subscribe(x =>
            {
                ErrorText = string.Empty;
                KickModel.UserId = x.UserId;
            });

            eventAggregator.GetEvent<SendEvent<(DetailedUserModel, string)>>().Subscribe(x =>
            {
                KickModel.UserId = x.Item1.UserId;
            }, ThreadOption.PublisherThread, false, x => x.Item2.Equals("KickControl"));

            panelOpened.Notify("KickControl");
        }

        public async void Execute()
        {
            ErrorText = string.Empty;
            if (KickModel.UserId == Guid.Empty) return;
            if (KickModel.UserId == staticData.UserData.UserId)
            {
                ErrorText = "You cannot kick yourself";
                return;
            }

            bool result = false;
            dialogService.ShowDialog("ConfirmControl", new DialogParameters
            {
                { "title", "Confirm Kick" },
                { "message", "Are you sure, you want to kick this user?" }
            }, x =>
            {
                result = x.Result switch
                {
                    ButtonResult.OK => true,
                    ButtonResult.Cancel => false,
                    _ => false,
                };
            });

            if (!result) return;

            IsButtonEnabled = false;

            try
            {
                await usersService_Hubs.Send(UserServiceHubs.Users, "KickUser", KickModel);
                ErrorText = "Kick has been sent";
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }
    }
}
