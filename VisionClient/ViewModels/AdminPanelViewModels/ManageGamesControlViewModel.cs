using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Utility;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ManageGamesControlViewModel : BindableBase
    {
        private object bannerImage = new();
        public object BannerImage
        {
            get { return bannerImage; }
            set { SetProperty(ref bannerImage, value); }
        }

        private object homeImage = new();
        public object HomeImage
        {
            get { return homeImage; }
            set { SetProperty(ref homeImage, value); }
        }

        private object iconImage = new();
        public object IconImage
        {
            get { return iconImage; }
            set { SetProperty(ref iconImage, value); }
        }

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

        private GameModel? selectedGame;
        public GameModel? SelectedGame
        {
            get { return selectedGame; }
            set 
            {
                var prop = value?.Clone() as GameModel;
                if(prop is not null)
                {
                    IconImage = prop.IconUrl;
                    HomeImage = prop.CoverUrl;
                    BannerImage = prop.BannerUrl;
                }

                SetProperty(ref selectedGame, prop); 
            }
        }

        private readonly IGamesRepository gamesRepository;
        private readonly IDialogService dialogService;

        public IStaticData StaticData { get; }
        public DelegateCommand ExecuteCommand { get; }
        public DelegateCommand ExecuteDeleteCommand { get; }
        public DelegateCommand<string> OpenImageCommand { get; }

        public ManageGamesControlViewModel(IStaticData staticData, IGamesRepository gamesRepository, IDialogService dialogService)
        {
            OpenImageCommand = new DelegateCommand<string>(OpenImage);
            ExecuteCommand = new DelegateCommand(Execute);
            ExecuteDeleteCommand = new DelegateCommand(DeleteGame);
            StaticData = staticData;
            this.gamesRepository = gamesRepository;
            this.dialogService = dialogService;
        }

        private void OpenImage(string imageContainer)
        {
            var image = FileDialogHelper.OpenFile(false);
            if (image.Any())
            {
                switch (imageContainer)
                {
                    case "Icon":
                        IconImage = image.First();
                        break;
                    case "Banner":
                        BannerImage = image.First();
                        break;
                    case "Home":
                        HomeImage = image.First();
                        break;
                    default:
                        break;
                }
            }
        }

        private async void DeleteGame()
        {
            if (SelectedGame is null || SelectedGame.Id == Guid.Empty) return;

            bool result = false;
            dialogService.ShowDialog("ConfirmControl", new DialogParameters
                {
                    { "title", "Confirm Delete" },
                    { "message", "Are you sure, you want to delete this game?" }
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
                (bool success, ErrorText) = await gamesRepository.DeleteGame(SelectedGame.Id);
                if (success)
                {
                    StaticData.GameModels.Remove(SelectedGame);
                    SelectedGame = new();
                }
                IsButtonEnabled = true;
            }
            catch(Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }

        private async void Execute()
        {
            ErrorText = string.Empty;
            if (SelectedGame is null) return;
            IsButtonEnabled = false;
            try
            {

                EditGameDto gameData = new();
                gameData.Informations = SelectedGame.Informations;
                gameData.Name = SelectedGame.Name;
                gameData.Requirements = SelectedGame.Requirements;
                gameData.IsAvailable = SelectedGame.IsAvailable;
                gameData.Id = SelectedGame.Id;

                if (IconImage.GetType() == typeof(BitmapImage))
                {
                    BitmapImage bitmapImage = (BitmapImage)IconImage;
                    gameData.IconPhoto = bitmapImage.GetBase64();
                }

                if (BannerImage.GetType() == typeof(BitmapImage))
                {
                    BitmapImage bitmapImage = (BitmapImage)BannerImage;
                    gameData.BannerPhoto = bitmapImage.GetBase64();
                }

                if (HomeImage.GetType() == typeof(BitmapImage))
                {
                    BitmapImage bitmapImage = (BitmapImage)HomeImage;
                    gameData.CoverPhoto = bitmapImage.GetBase64();
                }

                ErrorText = await gamesRepository.EditGame(gameData);
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
