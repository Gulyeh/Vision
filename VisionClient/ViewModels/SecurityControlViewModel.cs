using Prism.Commands;
using Prism.Mvvm;
using System;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;

namespace VisionClient.ViewModels
{
    internal class SecurityControlViewModel : BindableBase
    {
        private string newpassword = string.Empty;
        public string NewPassword
        {
            get { return newpassword; }
            set { SetProperty(ref newpassword, value); }
        }

        private string repeatpassword = string.Empty;
        public string RepeatPassword
        {
            get { return repeatpassword; }
            set { SetProperty(ref repeatpassword, value); }
        }

        private string currentPassword = string.Empty;
        public string CurrentPassword
        {
            get { return currentPassword; }
            set { SetProperty(ref currentPassword, value); }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private string newPassword_Error = string.Empty;
        public string NewPassword_Error
        {
            get { return newPassword_Error; }
            set { SetProperty(ref newPassword_Error, value); }
        }

        private string repeatPassword_Error = string.Empty;
        public string RepeatPassword_Error
        {
            get { return repeatPassword_Error; }
            set { SetProperty(ref repeatPassword_Error, value); }
        }

        private string currentPassword_Error = string.Empty;
        public string CurrentPassword_Error
        {
            get { return currentPassword_Error; }
            set { SetProperty(ref currentPassword_Error, value); }
        }

        public DelegateCommand SaveNewPasswordCommand { get; }
        private readonly IAccountRepository accountRepository;

        public SecurityControlViewModel(IAccountRepository accountRepository)
        {
            SaveNewPasswordCommand = new DelegateCommand(SaveNewPassword);
            this.accountRepository = accountRepository;
        }

        private bool Validators()
        {
            ResetErrors();

            if (!NewPassword.ValidatePassword()) NewPassword_Error = "Password requires minimum of 8 and maximum of 15 characters";
            if (!NewPassword.PasswordMatch(RepeatPassword)) RepeatPassword_Error = "Password does not match";
            if (!CurrentPassword.ValidatePassword()) CurrentPassword_Error = "Password requires minimum of 8 and maximum of 15 characters";

            return !string.IsNullOrEmpty(NewPassword_Error) && !string.IsNullOrEmpty(RepeatPassword_Error) &&
                !string.IsNullOrEmpty(CurrentPassword_Error);
        }

        private void ResetErrors()
        {
            ErrorText = string.Empty;
            CurrentPassword_Error = string.Empty;
            RepeatPassword_Error = string.Empty;
            NewPassword_Error = string.Empty;
        }

        private void ResetPasswordFields()
        {
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            RepeatPassword = string.Empty;
        }

        private async void SaveNewPassword()
        {
            try
            {
                if (Validators()) return;
                (bool isSuccess, string? Response) = await accountRepository.ChangePassword(CurrentPassword, NewPassword, RepeatPassword);
                if (isSuccess) ResetPasswordFields();
                ErrorText = string.IsNullOrEmpty(Response) ? string.Empty : Response;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
            }
        }
    }
}
