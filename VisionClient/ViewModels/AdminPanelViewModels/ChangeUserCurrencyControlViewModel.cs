using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using VisionClient.Core;
using VisionClient.Core.Enums;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ChangeUserCurrencyControlViewModel : BindableBase
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

        private int amount = 0;
        public int Amount
        {
            get { return amount; }
            set { SetProperty(ref amount, value); }
        }

        private Guid UserId { get; set; }
        private readonly IDialogService dialogService;
        private readonly IUsersRepository usersRepository;

        public DelegateCommand<string> ExecuteCommand { get; }

        public ChangeUserCurrencyControlViewModel(IEventAggregator eventAggregator, ITextEventHelper panelOpened,
                IDialogService dialogService, IUsersRepository usersRepository)
        {
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.dialogService = dialogService;
            this.usersRepository = usersRepository;

            eventAggregator.GetEvent<SendEvent<DetailedUserModel>>().Subscribe(x =>
            {
                ErrorText = string.Empty;
                UserId = x.UserId;
            });

            eventAggregator.GetEvent<SendEvent<(DetailedUserModel, string)>>().Subscribe(x =>
            {
                UserId = x.Item1.UserId;
            }, ThreadOption.PublisherThread, false, x => x.Item2.Equals("ChangeUserCurrencyControl"));

            panelOpened.Notify("ChangeUserCurrencyControl");
        }

        public async void Execute(string type)
        {
            ErrorText = string.Empty;
            if (UserId == Guid.Empty || Amount < 1) return;

            bool result = false;
            dialogService.ShowDialog("ConfirmControl", new DialogParameters
            {
                { "title", $"Confirm {type}" },
                { "message", $"Are you sure, you want to {type.ToLower()} {Amount} Visions?" }
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
                if (type.Equals("Substract")) Amount *= -1; 
                ErrorText = await usersRepository.ChangeCurrency(UserId, Amount);
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
