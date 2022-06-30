using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using VisionClient.Core;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ManageProductsControlViewModel : BindableBase
    {
        private Guid selectedGameId;
        public Guid SelectedGameId
        {
            get { return selectedGameId; }
            set
            {
                SetProperty(ref selectedGameId, value);
                if (value != Guid.Empty) GetGamePackages();
            }
        }

        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set { SetProperty(ref loadingVisibility, value); }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }


        private readonly IGamesRepository gamesRepository;
        private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;

        public IStaticData StaticData { get; }
        public DelegateCommand<ProductsModel> EditProductCommand { get; }
        public DelegateCommand<Guid?> DeleteProductCommand { get; }
        public DelegateCommand ExecuteCommand { get; }
        public ObservableCollection<ProductsModel> ProductsList { get; }

        public ManageProductsControlViewModel(IStaticData staticData, IGamesRepository gamesRepository, IDialogService dialogService,
            IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            StaticData = staticData;
            this.gamesRepository = gamesRepository;
            this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;

            ProductsList = new();
            ExecuteCommand = new DelegateCommand(GetGamePackages);
            DeleteProductCommand = new DelegateCommand<Guid?>(DeleteProduct);
            EditProductCommand = new DelegateCommand<ProductsModel>(EditProduct);

            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                GetGamePackages();
            }, ThreadOption.PublisherThread, false, x => x.Equals("UpdatePackage"));
        }

        private async void GetGamePackages()
        {
            if (SelectedGameId == Guid.Empty) return;
            LoadingVisibility = Visibility.Visible;
            ErrorText = string.Empty;

            try
            {
                var packages = await gamesRepository.GetProducts(SelectedGameId);
                if (packages is null) throw new Exception();

                ProductsList.Clear();

                var gameProduct = new ProductsModel()
                {
                    Id = packages.GameId,
                    Price = packages.OldPrice is null ? packages.Price : (decimal)packages.OldPrice,
                    Discount = packages.Discount,
                    Details = packages.Details,
                    IsAvailable = packages.IsAvailable,
                    Title = packages.Title,
                    PhotoUrl = packages.PhotoUrl
                };

                packages.Products.Insert(0, gameProduct);

                foreach (var product in packages.Products)
                {
                    if (product.OldPrice is not null) product.Price = (decimal)product.OldPrice;
                    ProductsList.Add(product);
                }

                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                LoadingVisibility = Visibility.Collapsed;
                ErrorText = "Something went wrong";
            }
        }

        private async void DeleteProduct(Guid? packageId)
        {
            if (packageId is null || packageId == Guid.Empty || packageId == SelectedGameId) return;

            bool result = false;
            dialogService.ShowDialog("ConfirmControl", new DialogParameters
                {
                    { "title", "Confirm Delete" },
                    { "message", "Are you sure, you want to delete this product?" }
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

            ErrorText = string.Empty;
            LoadingVisibility = Visibility.Visible;

            try
            {
                (bool success, ErrorText) = await gamesRepository.DeleteProduct((Guid)packageId, SelectedGameId);
                if (success) ProductsList.Remove(ProductsList.First(x => x.Id == packageId));

                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                LoadingVisibility = Visibility.Collapsed;
                ErrorText = "Something went wrong";
            }
        }

        private void EditProduct(ProductsModel model)
        {
            regionManager.RequestNavigate("AdminPanelRegion", "EditPackageControl");
            eventAggregator.GetEvent<SendEvent<(ProductsModel, Guid)>>().Publish((model, SelectedGameId));
        }

    }
}
