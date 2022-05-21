using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisionClient.Core.Enums;
using VisionClient.Core.Events;
using VisionClient.Core.Models.Account;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services.IServices;
using VisionClient.Extensions;
using VisionClient.Helpers;
using VisionClient.Utility;
using VisionClient.Views;
using VisionClient.Views.Login.Dialogs;

namespace VisionClient.ViewModels
{
    internal class LoginControlViewModel : BindableBase
    {
        private string? errorText;
        public string? ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private bool keepLogged = false;
        public bool KeepLogged
        {
            get { return keepLogged; }
            set { SetProperty(ref keepLogged, value); }
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

        private string? authCode;
        public string? AuthCode
        {
            get { return authCode; }
            set { SetProperty(ref authCode, value); }
        }

        public UserControl? tempControl { get; set; } = null;
        public DelegateCommand RegisterCommand { get; set; }
        public DelegateCommand ForgotPasswordCommand { get; set; }
        public DelegateCommand LoginCommand { get; set; }

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IDialogService dialogService;
        private readonly IAccountRepository accountRepository;
        private readonly IXMLCredentials xmlCredentials;

        public LoginControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, 
            IDialogService dialogService, IAccountRepository accountRepository, IXMLCredentials xmlCredentials)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.accountRepository = accountRepository;
            this.xmlCredentials = xmlCredentials;

            RegisterCommand = new DelegateCommand(SwitchToRegister);
            ForgotPasswordCommand = new DelegateCommand(OpenForgotPasswordDialog);
            LoginCommand = new DelegateCommand(ExecuteLogin);

            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                AuthCode = x.Replace("authCode:", "");
                ExecuteLogin();
                AuthCode = string.Empty;
            }, ThreadOption.PublisherThread, false, x => x.Contains("authCode:"));


            if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
            {
                (Email, Password, KeepLogged) = xmlCredentials.LoadCredentials();
                if (KeepLogged) LoginCommand.Execute();
            }
        }

        private bool Validators()
        {
            Email_ErrorText = string.Empty;
            Password_ErrorText = string.Empty;
            ErrorText = string.Empty;

            if (!Email.ValidateEmailAddress())
            {
                Email_ErrorText = "Email address is invalid";
            }

            if (!Password.ValidatePassword())
            {
                Password_ErrorText = "Password requires minimum of 8 and maximum of 15 characters";
            }

            return string.IsNullOrEmpty(Password_ErrorText) && string.IsNullOrEmpty(Email_ErrorText);
        }

        private async void ExecuteLogin()
        {
            var loginVis = new LoginWindowLoadingVisibilityHelper(eventAggregator);
            try
            {
                if (!Validators()) return;
                loginVis.IsLoadingVisible(true);

                LoginResponse loginResponse = await accountRepository.LoginUser(Email, Password, AuthCode);
                string? stringData = loginResponse.Data.ToString()?.Replace("[", "").Replace("]", "");

                if (!string.IsNullOrEmpty(stringData))
                {
                    _ = loginResponse.ResponseType switch
                    {
                        LoginResponseTypes.WrongCredentials => WrongCredentials(stringData),
                        LoginResponseTypes.UserBanned => UserBanned(stringData),
                        LoginResponseTypes.TwoFactorAuth => TFARequired(),
                        LoginResponseTypes.WrongAuthCode => WrongAuthCode(stringData),
                        _ => LoginPassed()
                    };

                    xmlCredentials.SaveCredentials(Email, Password, KeepLogged);
                    loginVis.IsLoadingVisible(false);
                }
                else throw new Exception();
            }
            catch (Exception) {
                loginVis.IsLoadingVisible(false);
                ErrorText = "Wrong email or password"; 
            };          
        }

        private bool WrongCredentials(string stringData)
        {
            ErrorText = JsonConvert.DeserializeObject<string>(stringData);
            return true;
        }

        private bool UserBanned(string stringData)
        {
            dialogService.ShowDialog("UserBannedControl", new DialogParameters()
            {
                { "content",  stringData }
            }, null);
            return true;
        }

        private bool TFARequired()
        {
            dialogService.ShowDialog(nameof(TFAControl));
            return true;
        }

        private bool WrongAuthCode(string stringData)
        {
            dialogService.ShowDialog(nameof(TFAControl), new DialogParameters()
            {
                { "content", JsonConvert.DeserializeObject<string>(stringData) }
            }, null);
            return true;
        }

        private bool LoginPassed()
        {
            var bs = new LoadingBootstrapper();
            bs.Run();
            CloseParentWindowHelper.Close(tempControl);
            return true;
        }

        private void OpenForgotPasswordDialog()
        {
            dialogService.ShowDialog("ForgotPasswordControl");
        }

        private void SwitchToRegister()
        {
            regionManager.RequestNavigate("LoginContent", "RegisterControl");
        }
    }
}
