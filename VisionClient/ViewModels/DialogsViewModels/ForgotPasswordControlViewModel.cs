using Prism.Events;
using Prism.Services.Dialogs;
using System;
using VisionClient.Core.Events;
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

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private bool isEnabledButton = true;
        public bool IsEnabledButton
        {
            get { return isEnabledButton; }
            set { SetProperty(ref isEnabledButton, value); }
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IAccountRepository accountRepository;
        private readonly IDialogService dialogService;

        public ForgotPasswordControlViewModel(IEventAggregator eventAggregator, IAccountRepository accountRepository,
            IDialogService dialogService) : base(eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.accountRepository = accountRepository;
            this.dialogService = dialogService;
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            eventAggregator.GetEvent<SendEvent<string>>().Publish("ShadowLoginWindow");
        }

        public override void OnDialogClosed()
        {
            eventAggregator.GetEvent<SendEvent<string>>().Publish("ShadowLoginWindow");
        }

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

        protected override async void Execute(object? data)
        {
            try
            {
                if (!Validators()) return;
                IsEnabledButton = false;

                (bool isSuccess, string? response) = await accountRepository.RequestPasswordReset(Email);
                if (!isSuccess)
                {
                    ErrorText = string.IsNullOrEmpty(response) ? string.Empty : response;
                    IsEnabledButton = true;
                    return;
                }

                RaiseRequestClose(new DialogResult(ButtonResult.OK));

                dialogService.ShowDialog("InformationControl", new DialogParameters()
                {
                    { "title", "Password recovery" },
                    { "message", response }
                }, null);
            }
            catch (Exception)
            {
                IsEnabledButton = true;
                ErrorText = "Something went wrong";
            }
        }
    }
}
