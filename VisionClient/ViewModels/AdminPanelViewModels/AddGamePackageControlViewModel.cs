using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Windows.Media.Imaging;
using VisionClient.Core;
using VisionClient.Core.Dtos;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Utility;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class AddGamePackageControlViewModel : BindableBase
    {
        private object coverImage = new();
        public object CoverImage
        {
            get { return coverImage; }
            set { SetProperty(ref coverImage, value); }
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

        private GameModel selectedGame = new();
        public GameModel SelectedGame
        {
            get { return selectedGame; }
            set { SetProperty(ref selectedGame, value); }
        }

        private AddPackageDto packageData = new();
        public AddPackageDto PackageData
        {
            get { return packageData; }
            set { SetProperty(ref packageData, value); }
        }

        private readonly IGamesRepository gamesRepository;

        public DelegateCommand OpenImageCommand { get; }
        public DelegateCommand ExecuteCommand { get; }
        public IStaticData StaticData { get; }

        public AddGamePackageControlViewModel(IStaticData StaticData, IGamesRepository gamesRepository)
        {
            ExecuteCommand = new DelegateCommand(Execute);
            OpenImageCommand = new DelegateCommand(OpenImage);
            this.StaticData = StaticData;
            this.gamesRepository = gamesRepository;
            ClearData();
        }

        private void OpenImage()
        {
            var image = FileDialogHelper.OpenFile(false);
            if (image.Any()) CoverImage = image.First();
        }

        private bool Validator()
        {
            if (SelectedGame.Id != Guid.Empty && PackageData.Validator() && CoverImage.GetType() == typeof(BitmapImage)) return true;
            ErrorText = "Please fill all data";
            return false;
        }

        private async void Execute()
        {
            ErrorText = string.Empty;
            if (!Validator()) return;
            IsButtonEnabled = false;
            try
            {
                BitmapImage cover = (BitmapImage)CoverImage;
                PackageData.GameId = SelectedGame.Id;
                PackageData.Photo = cover.GetBase64();

                (bool success, ErrorText) = await gamesRepository.AddGamePackage(PackageData);
                IsButtonEnabled = true;
                if (success) ClearData();
            }
            catch (Exception)
            {
                IsButtonEnabled = true;
                ErrorText = "Something went wrong";
            }
        }

        private void ClearData()
        {
            CoverImage = "pack://application:,,,/Images/AddBanner.png";
            PackageData = new();
            SelectedGame = new();
        }
    }
}
