using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VisionClient.Core.Dtos;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Utility;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class EditPackageControlViewModel : BindableBase
    {
        private bool isGame = false;
        public bool IsGame
        {
            get { return isGame; }
            set { SetProperty(ref isGame, !value); }
        }

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

        private EditPackageDto packageData = new();
        public EditPackageDto PackageData
        {
            get { return packageData; }
            set { SetProperty(ref packageData, value); }
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly IGamesRepository gamesRepository;

        public DelegateCommand OpenImageCommand { get; }
        public DelegateCommand ExecuteCommand { get; }
        public DelegateCommand BackwardCommand { get; }

        public EditPackageControlViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IGamesRepository gamesRepository)
        {
            ExecuteCommand = new DelegateCommand(Execute);
            OpenImageCommand = new DelegateCommand(OpenImage);
            BackwardCommand = new DelegateCommand(NavigateToProducts);

            eventAggregator.GetEvent<SendEvent<(ProductsModel, Guid)>>().Subscribe(x =>
            {
                IsGame = x.Item2.Equals(x.Item1.Id);

                var editPackage = new EditPackageDto()
                {
                    Id = x.Item1.Id,
                    Title = x.Item1.Title,
                    Price = x.Item1.Price,
                    Details = x.Item1.Details,
                    Discount = x.Item1.Discount,
                    IsAvailable = x.Item1.IsAvailable,
                    GameId = x.Item2.Equals(x.Item1.Id) ? Guid.Empty : x.Item2
                };

                PackageData = editPackage;
                CoverImage = x.Item1.PhotoUrl;
            });

            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.gamesRepository = gamesRepository;
        }

        private void OpenImage()
        {
            var image = FileDialogHelper.OpenFile(false);
            if (image.Any()) CoverImage = image.First();
        }

        private async void Execute()
        {
            ErrorText = string.Empty;
            if (!PackageData.Validator())
            {
                ErrorText = "Please fill all data";
                return;
            }
            IsButtonEnabled = false;

            try
            {
                if (CoverImage.GetType() == typeof(BitmapImage)) {
                    BitmapImage image = (BitmapImage)CoverImage;
                    PackageData.Photo = image.GetBase64();
                }

                (bool success, ErrorText) = await gamesRepository.EditPackage(PackageData);
                if (success)
                {
                    eventAggregator.GetEvent<SendEvent<string>>().Publish("UpdatePackage");
                    NavigateToProducts();
                }
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                IsButtonEnabled = true;
                ErrorText = "Something went wrong";
            }
        }

        private void NavigateToProducts() => regionManager.RequestNavigate("AdminPanelRegion", "ManageProductsControl");
    }
}
