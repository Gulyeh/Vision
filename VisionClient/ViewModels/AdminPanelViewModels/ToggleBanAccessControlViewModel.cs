using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ToggleBanAccessControlViewModel : BindableBase
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

        private BanModelDto banModel = new();
        public BanModelDto BanModel
        {
            get { return banModel; }
            set { SetProperty(ref banModel, value); }
        }
        private bool isBanned = false;
        public bool IsBanned
        {
            get => isBanned;
            set { SetProperty(ref isBanned, value); }
        }

        private readonly IDialogService dialogService;
        private readonly IAccountRepository accountRepository;
        private readonly IStaticData staticData;

        public DelegateCommand ExecuteCommand { get; }
        public DelegateCommand ExecuteUnbanCommand { get; }

        public ToggleBanAccessControlViewModel(IEventAggregator eventAggregator, ITextEventHelper eventHelper, IDialogService dialogService, IAccountRepository accountRepository, IStaticData staticData)
        {
            ExecuteCommand = new DelegateCommand(BanUser);
            ExecuteUnbanCommand = new DelegateCommand(UnbanUser);

            eventAggregator.GetEvent<SendEvent<(DetailedUserModel, string)>>().Subscribe(x =>
            {
                BanModel.UserId = x.Item1.UserId;
                IsBanned = x.Item1.IsBanned;
            }, ThreadOption.PublisherThread, false, x => x.Item2.Equals("ToggleBanAccess"));

            eventAggregator.GetEvent<SendEvent<DetailedUserModel>>().Subscribe(x =>
            {
                ErrorText = string.Empty;
                BanModel = new();
                BanModel.UserId = x.UserId;
                IsBanned = x.IsBanned;
            });

            eventHelper.Notify("ToggleBanAccess");
            this.dialogService = dialogService;
            this.accountRepository = accountRepository;
            this.staticData = staticData;
        }

        private async void UnbanUser()
        {
            ErrorText = string.Empty;
            if (BanModel.UserId == Guid.Empty || !OpenDialog("Unban user", "Are you sure, you want to unban this user?")) return;
            IsButtonEnabled = false;

            try
            {
                (bool success, ErrorText) = await accountRepository.UnbanUser(BanModel.UserId);
                if (success) IsBanned = false;
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }

        private async void BanUser()
        {
            ErrorText = string.Empty;
            if (!BanModel.Validation())
            {
                ErrorText = "Please fill all needed data";
                return;
            }
            else if (BanModel.UserId == staticData.UserData.UserId)
            {
                ErrorText = "You cannot ban yourself";
                return;
            }
            else if (!OpenDialog("Ban user", "Are you sure, you want to ban user?")) return;
            IsButtonEnabled = false;

            try
            {
                (bool success, ErrorText) = await accountRepository.BanUser(BanModel);
                if (success) IsBanned = true;
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }

        private bool OpenDialog(string title, string message)
        {
            bool result = false;
            dialogService.ShowDialog("ConfirmControl", new DialogParameters
            {
                { "title", title },
                { "message", message }
            }, x =>
            {
                result = x.Result switch
                {
                    ButtonResult.OK => true,
                    ButtonResult.Cancel => false,
                    _ => false,
                };
            });

            return result;
        }

    }
}
