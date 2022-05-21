using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class ForgotPasswordControlViewModel : DialogHelper
    {
        private string email = string.Empty;
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        private string? errorText;
        public string? ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private readonly IAccountRepository accountRepository;
        private readonly IDialogService dialogService;

        public ForgotPasswordControlViewModel(IEventAggregator eventAggregator, IAccountRepository accountRepository,
            IDialogService dialogService) : base(eventAggregator)
        {
            this.accountRepository = accountRepository;
            this.dialogService = dialogService;
        }

        public override void OnDialogOpened(IDialogParameters parameters){}

        public override void OnDialogClosed(){}

        private bool Validators()
        {
            ErrorText = string.Empty;

            if (!Email.ValidateEmailAddress())
            {
                ErrorText = "Email Address is invalid";
                return false;
            }

            return true;
        }

        public override async void Execute(object? data)
        {
            try
            {
                if (!Validators()) return;
                (bool isSuccess, string? response) = await accountRepository.RequestPasswordReset(Email);
                if (!isSuccess)
                {
                    ErrorText = response;
                    return;
                }

                dialogService.ShowDialog("InformationControl", new DialogParameters()
                {
                    { "title", "Password recovery" },
                    { "message", response }
                }, null);

                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
            }
        }
    }
}
