using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using VisionClient.Core.Enums;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Models.Account;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Helpers;
using VisionClient.Utility;
using VisionClient.Views.Login.Dialogs;

namespace VisionClient.ViewModels
{
    internal class LoginControlViewModel : BindableBase
    {
        private string errorText = string.Empty;
        public string ErrorText
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

        private string email_errorText = string.Empty;
        public string Email_ErrorText
        {
            get { return email_errorText; }
            set { SetProperty(ref email_errorText, value); }
        }

        private string password_errorText = string.Empty;
        public string Password_ErrorText
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

        private string authCode = string.Empty;
        public string AuthCode
        {
            get { return authCode; }
            set { SetProperty(ref authCode, value); }
        }

        public UserControl? TempControl { get; set; } = null;
        public DelegateCommand RegisterCommand { get; }
        public DelegateCommand ForgotPasswordCommand { get; }
        public DelegateCommand LoginCommand { get; }

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IDialogService dialogService;
        private readonly IAccountRepository accountRepository;

        public LoginControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,
            IDialogService dialogService, IAccountRepository accountRepository)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.accountRepository = accountRepository;

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
                (Email, Password, KeepLogged) = XMLCredentials.LoadCredentials();
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
                        LoginResponseTypes.TwoFactorAuth => TFARequired(loginVis),
                        LoginResponseTypes.WrongAuthCode => WrongAuthCode(stringData),
                        _ => await LoginPassed()
                    };

                    XMLCredentials.SaveCredentials(Email, Password, KeepLogged);
                    loginVis.IsLoadingVisible(false);
                }
                else throw new Exception();
            }
            catch (Exception)
            {
                loginVis.IsLoadingVisible(false);
                ErrorText = "Wrong email or password";
            };
        }

        private bool WrongCredentials(string stringData)
        {
            var json = JsonConvert.DeserializeObject<string>(stringData);
            if (json is not null) ErrorText = json;
            return true;
        }

        private bool UserBanned(string stringData)
        {
            var json = JsonConvert.DeserializeObject<BanModel>(stringData);
            dialogService.ShowDialog("UserBannedControl", new DialogParameters()
            {
                { "banModel",  json }
            }, null);
            return true;
        }

        private bool TFARequired(LoginWindowLoadingVisibilityHelper loginVis)
        {
            loginVis.IsLoadingVisible(false);
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

        private async Task<bool> LoginPassed()
        {
            bool response = await accountRepository.GetServerData();
            if (!response) throw new Exception();

            var bs = new LoadingBootstrapper();
            bs.Run();
            CloseParentWindowHelper.Close(TempControl);
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
