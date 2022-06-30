using Prism.Events;
using Prism.Services.Dialogs;
using System;
using VisionClient.Core;
using VisionClient.Core.Events;
using VisionClient.Core.Models.Account;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Helpers;
using VisionClient.Views.Login.Dialogs;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class DeleteAccountControlViewModel : DialogHelper
    {
        private LoginModel loginModel = new();
        public LoginModel LoginModel
        {
            get { return loginModel; }
            set { SetProperty(ref loginModel, value); }
        }

        private bool isButtonEnabled = true;
        public bool IsButtonEnabled
        {
            get { return isButtonEnabled; }
            set { SetProperty(ref isButtonEnabled, value); }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private readonly IAccountRepository accountRepository;
        private readonly IDialogService dialogService;

        public DeleteAccountControlViewModel(IEventAggregator eventAggregator, IAccountRepository accountRepository,
            IStaticData staticData, IDialogService dialogService) : base(eventAggregator)
        {
            this.accountRepository = accountRepository;
            this.dialogService = dialogService;
            LoginModel.Email = staticData.UserData.Email;

            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                LoginModel.AuthCode = x.Replace("authCode:", "");
                Execute(null);
            }, ThreadOption.PublisherThread, false, x => x.Contains("authCode:"));
        }

        protected override async void Execute(object? data)
        {
            ErrorText = string.Empty;
            if (LoginModel.Password.Length < 8)
            {
                ErrorText = "Password requires at least 8 and maximum 15 characters";
                return;
            }
            IsButtonEnabled = false;
            try
            {
                (ErrorText, int status) = await accountRepository.DeleteAccount(LoginModel);
                if (status == 403) dialogService.ShowDialog(nameof(TFAControl));
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
