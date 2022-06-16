using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows.Media;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Utility;

namespace VisionClient.ViewModels
{
    internal class AuthenticationControlViewModel : BindableBase
    {
        private string securityCode = string.Empty;
        public string SecurityCode
        {
            get { return securityCode; }
            set { SetProperty(ref securityCode, value); }
        }

        private ImageSource? qrCodeImage = null;
        public ImageSource? QRCodeImage
        {
            get { return qrCodeImage; }
            set { SetProperty(ref qrCodeImage, value); }
        }

        private string tokenCode = string.Empty;
        public string TokenCode
        {
            get { return tokenCode; }
            private set { SetProperty(ref tokenCode, value); }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            private set { SetProperty(ref errorText, value); }
        }

        private string codeError = string.Empty;
        public string CodeError
        {
            get { return codeError; }
            private set { SetProperty(ref codeError, value); }
        }

        public DelegateCommand<string> GetAppCommand { get; }
        public DelegateCommand ToggleAuthCommand { get; }
        private readonly IAccountRepository accountRepository;

        public AuthenticationControlViewModel(IAccountRepository accountRepository)
        {
            GetAppCommand = new DelegateCommand<string>(GetApp);
            ToggleAuthCommand = new DelegateCommand(ToggleAuth);
            this.accountRepository = accountRepository;
            Get2FAData();
        }

        private void GetApp(string name)
        {
            switch (name)
            {
                case "Google":
                    OpenBrowserHelper.OpenUrl("https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2");
                    break;
                case "Apple":
                    OpenBrowserHelper.OpenUrl("https://apps.apple.com/us/app/google-authenticator/id388497605");
                    break;
                default:
                    break;
            }
        }

        private async void Get2FAData()
        {
            TokenCode = string.Empty;
            QRCodeImage = null;

            try
            {
                (bool isSuccess, object? response) = await accountRepository.Generate2FA();
                if (!isSuccess)
                {
                    var text = response as string;
                    ErrorText = string.IsNullOrEmpty(text) ? "Something went wrong" : text;
                    return;
                }

                if (response is not TFADataModel responseModel) throw new Exception();

                TokenCode = responseModel.TokenCode;
                QRCodeImage = GenerateQR.Generate(responseModel.QrCodeUri);
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
            }
        }

        private async void ToggleAuth()
        {
            ErrorText = string.Empty;
            CodeError = string.Empty;

            if (string.IsNullOrEmpty(SecurityCode))
            {
                CodeError = "Field cannot be empty";
                return;
            }
            else if (SecurityCode.Length != 6)
            {
                CodeError = "6 digits are required";
                return;
            }

            try
            {
                (bool isSuccess, string? response) = await accountRepository.Toggle2FA(SecurityCode);
                if (!isSuccess)
                {
                    ErrorText = string.IsNullOrEmpty(response) ? "Something went wrong" : response;
                    return;
                }

                SecurityCode = string.Empty;
                ErrorText = string.IsNullOrEmpty(response) ? "Two-factor authentication has been changed" : response;
                Get2FAData();
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
            }
        }
    }
}
