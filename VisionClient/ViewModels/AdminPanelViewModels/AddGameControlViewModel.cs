using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VisionClient.Core.Dtos;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Utility;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class AddGameControlViewModel : BindableBase
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

        private AddGameDto gameModel = new();
        public AddGameDto GameModel
        {
            get { return gameModel; }
            set { SetProperty(ref gameModel, value); }
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

        private readonly IGamesRepository gamesRepository;
        public DelegateCommand<string> OpenImageCommand { get; }
        public DelegateCommand ExecuteCommand { get; }

        public AddGameControlViewModel(IGamesRepository gamesRepository)
        {
            ExecuteCommand = new DelegateCommand(Execute);
            OpenImageCommand = new DelegateCommand<string>(OpenImage);
            ClearData();
            this.gamesRepository = gamesRepository;
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

        private bool Validator()
        {
            ErrorText = string.Empty;
            if (HomeImage.GetType() != typeof(BitmapImage) || IconImage.GetType() != typeof(BitmapImage) || BannerImage.GetType() != typeof(BitmapImage) || !GameModel.Validation())
            {
                ErrorText = "Please fill all fields";
                return false;
            }
            return true;
        }

        private async void Execute()
        {
            ErrorText = string.Empty;
            if (!Validator()) return;
            IsButtonEnabled = false;
            try
            {
                BitmapImage banner = (BitmapImage)BannerImage;
                BitmapImage home = (BitmapImage)HomeImage;
                BitmapImage icon = (BitmapImage)IconImage;

                GameModel.BannerPhoto = banner.GetBase64();
                GameModel.CoverPhoto = home.GetBase64();
                GameModel.IconPhoto = icon.GetBase64();

                (bool success, ErrorText) = await gamesRepository.AddGame(GameModel);
                IsButtonEnabled = true;
                if(success) ClearData();
            }
            catch (Exception)
            {
                IsButtonEnabled = true;
                ErrorText = "Something went wrong";
            }
        }

        private void ClearData()
        {
            BannerImage = "pack://application:,,,/Images/AddBanner.png";
            HomeImage = "pack://application:,,,/Images/AddHomeBanner.png";
            IconImage = "pack://application:,,,/Images/AddIcon.png";
            GameModel = new();
        }

    }
}
