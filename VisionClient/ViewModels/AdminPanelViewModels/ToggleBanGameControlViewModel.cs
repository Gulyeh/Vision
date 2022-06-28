using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ToggleBanGameControlViewModel : BindableBase
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

        private BanGameDto banModel = new();
        public BanGameDto BanModel
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
        private readonly IGamesRepository gamesRepository;

        public IStaticData StaticData { get; }
        public DelegateCommand ExecuteCommand { get; }
        public DelegateCommand ExecuteUnbanCommand { get; }
        public DelegateCommand CheckUserIsBannedCommand { get; }

        public ToggleBanGameControlViewModel(IStaticData staticData, IEventAggregator eventAggregator, IDialogService dialogService, ITextEventHelper textEventHelper, IGamesRepository gamesRepository)
        {
            ExecuteCommand = new DelegateCommand(BanUser);
            ExecuteUnbanCommand = new DelegateCommand(UnbanUser);
            CheckUserIsBannedCommand = new DelegateCommand(CheckUserIsBanned);
            eventAggregator.GetEvent<SendEvent<(DetailedUserModel, string)>>().Subscribe(x =>
            {
                BanModel.UserId = x.Item1.UserId;
            }, ThreadOption.PublisherThread, false, x => x.Item2.Equals("ToggleBanGame"));

            eventAggregator.GetEvent<SendEvent<DetailedUserModel>>().Subscribe(x => {
                BanModel = new();
                ErrorText = string.Empty;
                BanModel.UserId = x.UserId;
            });

            textEventHelper.Notify("ToggleBanGame");

            StaticData = staticData;
            this.dialogService = dialogService;
            this.gamesRepository = gamesRepository;
        }

        private async void CheckUserIsBanned()
        {
            ErrorText = string.Empty;
            if (BanModel.UserId == Guid.Empty || BanModel.GameId == Guid.Empty) return;
            IsButtonEnabled = false;

            try
            {
                (bool isbanned, string response) = await gamesRepository.CheckIfUserIsBanned(BanModel.UserId, BanModel.GameId);
                if (!string.IsNullOrWhiteSpace(response)) ErrorText = response;
                else IsBanned = isbanned;

                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }

        private async void UnbanUser()
        {
            ErrorText = string.Empty;
            if (BanModel.UserId == Guid.Empty || BanModel.GameId == Guid.Empty
                || !OpenDialog("Unban user", "Are you sure, you want to unban this user?")) return;
            IsButtonEnabled = false;

            try
            {
                (bool success, ErrorText) = await gamesRepository.UnbanUser(BanModel.UserId, BanModel.GameId);
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
            else if (BanModel.UserId == StaticData.UserData.UserId)
            {
                ErrorText = "You cannot ban yourself";
                return;
            }
            else if (!OpenDialog("Ban user", "Are you sure, you want to ban user from this game?")) return;
            IsButtonEnabled = false;

            try
            {
                (bool success, ErrorText) = await gamesRepository.BanUser(BanModel);
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
