using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Helpers;

namespace VisionClient.ViewModels
{
    internal class RegisterControlViewModel : BindableBase
    {
        private string email = string.Empty;
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        private string password = string.Empty;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        private string repeatPassword = string.Empty;
        public string RepeatPassword
        {
            get { return repeatPassword; }
            set { SetProperty(ref repeatPassword, value); }
        }

        private string? email_errorText;
        public string? Email_ErrorText
        {
            get { return email_errorText; }
            set { SetProperty(ref email_errorText, value); }
        }

        private string? password_errorText;
        public string? Password_ErrorText
        {
            get { return password_errorText; }
            set { SetProperty(ref password_errorText, value); }
        }

        private string? repeatPassword_errorText;
        public string? RepeatPassword_ErrorText
        {
            get { return repeatPassword_errorText; }
            set { SetProperty(ref repeatPassword_errorText, value); }
        }

        private string? errorText;
        public string? ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private readonly IRegionManager regionManager;
        private readonly IAccountRepository accountRepository;
        private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;

        public DelegateCommand GoBackwardCommand { get; set; }
        public DelegateCommand RegisterCommand { get; set; }
        public RegisterControlViewModel(IRegionManager regionManager, IAccountRepository accountRepository,
            IDialogService dialogService, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.accountRepository = accountRepository;
            this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;
            GoBackwardCommand = new DelegateCommand(GoBackward);
            RegisterCommand = new DelegateCommand(RegisterExecute);
        }

        private bool Validators()
        {
            Email_ErrorText = string.Empty;
            Password_ErrorText = string.Empty;
            RepeatPassword_ErrorText = string.Empty;
            ErrorText = string.Empty;

            if (!Email.ValidateEmailAddress())
            {
                Email_ErrorText = "Email address is invalid";
            }

            if (!Password.ValidatePassword())
            {
                Password_ErrorText = "Password requires minimum of 8 and maximum of 15 characters";
            }

            if(Password != RepeatPassword)
            {
                RepeatPassword_ErrorText = "Passwords do not match";
            }

            return string.IsNullOrEmpty(Password_ErrorText) && string.IsNullOrEmpty(Email_ErrorText) && string.IsNullOrEmpty(RepeatPassword_ErrorText);
        }

        private async void RegisterExecute()
        {
            var loginVis = new LoginWindowLoadingVisibilityHelper(eventAggregator);
            try
            {
                if (!Validators()) return;
                loginVis.IsLoadingVisible(true);

                (bool IsSuccess, string? response) = await accountRepository.RegisterUser(Email, Password, RepeatPassword);
                if (!IsSuccess)
                {
                    ErrorText = response;
                    loginVis.IsLoadingVisible(false);
                    return;
                }

                loginVis.IsLoadingVisible(false);
                regionManager.RequestNavigate("LoginContent", "LoginControl");
                ResetVariables();
                dialogService.ShowDialog("InformationControl", new DialogParameters()
                    {
                        { "title", "Account Registered" },
                        { "message", response }
                    }, null);
            }
            catch (Exception)
            {
                loginVis.IsLoadingVisible(false);
                ErrorText = "Something went wrong";
            }
        }

        private void ResetVariables()
        {
            Email = string.Empty;
            Password = string.Empty;
            RepeatPassword = string.Empty;
            ErrorText = string.Empty;
            Email_ErrorText = string.Empty;
            Password_ErrorText = string.Empty;
            RepeatPassword_ErrorText = string.Empty;
        }

        private void GoBackward()
        {
            regionManager.RequestNavigate("LoginContent", "LoginControl");
        }
    }
}
