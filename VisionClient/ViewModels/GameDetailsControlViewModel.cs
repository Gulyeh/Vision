using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VisionClient.Core.Builders;
using VisionClient.Core.Enums;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.SignalR;

namespace VisionClient.ViewModels
{
    internal class GameDetailsControlViewModel : BindableBase
    {
        private GameModel gameModel = new();
        public GameModel GameModel
        {
            get { return gameModel; }
            set { SetProperty(ref gameModel, value); }
        }

        private ProductsModel selectedPackage = new();
        public ProductsModel SelectedPackage
        {
            get { return selectedPackage; }
            set
            {
                if (value is not null)
                {
                    SetProperty(ref selectedPackage, value);
                    NavigateToPurchase("PackageButton");
                }
            }
        }

        private GameProductModel gameProducts = new();
        public GameProductModel GameProducts
        {
            get { return gameProducts; }
            set { SetProperty(ref gameProducts, value); }
        }

        private bool mainButtonEnabled = true;
        public bool MainButtonEnabled
        {
            get { return mainButtonEnabled; }
            set { SetProperty(ref mainButtonEnabled, value); }
        }

        private NewsModel selectedNews = new();
        public NewsModel SelectedNews
        {
            get { return selectedNews; }
            set
            {
                SetProperty(ref selectedNews, value);
                Goto_NewsControl(selectedNews);
            }
        }

        private bool enabledProducts = true;
        public bool EnabledProducts
        {
            get { return enabledProducts; }
            set { SetProperty(ref enabledProducts, value); }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        public ObservableCollection<NewsModel> NewsList { get; }
        public DelegateCommand MainButtonCommand { get; }
        public DelegateCommand OpenCouponDialogCommand { get; }
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;
        private readonly IGamesRepository gamesRepository;

        public GameDetailsControlViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService,
            IGamesRepository gamesRepository, IUsersService_Hubs usersService_Hubs)
        {
            NewsList = new ObservableCollection<NewsModel>();
            OpenCouponDialogCommand = new DelegateCommand(OpenCouponDialog);
            MainButtonCommand = new DelegateCommand(MainButtonPressed);
            eventAggregator.GetEvent<SendEvent<GameModel>>().Subscribe(EventReceived);
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.gamesRepository = gamesRepository;
            usersService_Hubs.CouponProductEvent += UsedProductCoupon;

            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(async (x) =>
            {
                await GetProducts();
            }, ThreadOption.PublisherThread, false, x => x.Equals("ProductPurchased"));

            usersService_Hubs.CouponProductEvent += async (s, e) =>
            {
                if (GameProducts.GameId != e.Data.GameId) return;
                await GetProducts();
            };
        }

        private void UsedProductCoupon(object? sender, CouponProductEventArgs e)
        {
            if (!e.Data.IsSuccess) return;
            if (e.Data.GameId != Guid.Empty && GameModel.Id != e.Data.GameId) return;
            else if (e.Data.GameId == Guid.Empty && GameModel.Id != e.Data.ProductId) return;

            if (e.Data.GameId != Guid.Empty)
            {
                var product = GameProducts.Products.FirstOrDefault(x => x.Id == e.Data.ProductId);
                if (product is null) return;

                product.IsAvailable = false;
                return;
            }

            GameProducts.IsPurchased = true;
            EnabledProducts = true;
        }

        private async void MainButtonPressed()
        {
            switch (GameProducts.IsPurchased)
            {
                case true:
                    MainButtonEnabled = false;
                    await CheckGameAccess(GameModel.Id);
                    break;
                default:
                    NavigateToPurchase("MainButton");
                    break;
            }
        }

        private async Task CheckGameAccess(Guid gameId)
        {
            try
            {
                var response = await gamesRepository.CheckGameAccess(gameId);
                if (response is null)
                {
                    await Task.Delay(5000);
                    MainButtonEnabled = true;
                    return;
                }

                MainButtonEnabled = true;
                dialogService.ShowDialog("UserBannedControl", new DialogParameters()
                {
                    { "banModel",  response }
                }, null);
            }
            catch (Exception)
            {
                MainButtonEnabled = true;
                dialogService.ShowDialog("InformationControl", new DialogParameters
                {
                    { "title", "Error" },
                    { "message", "Could not obtain access data" }
                }, null);
            }
        }

        private async void NavigateToPurchase(string data)
        {
            var builder = new PurchaseModelBuilder();
            switch (data)
            {
                case "MainButton":
                    if (GameProducts is null || GameProducts.GameId == Guid.Empty) return;
                    builder.SetId(GameProducts.GameId);
                    builder.SetPrice(GameProducts.Price);
                    builder.SetTitle(GameProducts.Title);
                    builder.SetDiscount(GameProducts.Discount);
                    builder.SetPhotoUrl(GameProducts.PhotoUrl);
                    builder.SetOldPrice(GameProducts.OldPrice);
                    builder.SetDetails(GameProducts.Details);
                    builder.SetOrderType(OrderType.Game);
                    break;
                case "PackageButton":
                    if (SelectedPackage is null || SelectedPackage.Id == Guid.Empty) return;
                    builder.SetId(SelectedPackage.Id);
                    builder.SetPrice(SelectedPackage.Price);
                    builder.SetTitle(SelectedPackage.Title);
                    builder.SetDiscount(SelectedPackage.Discount);
                    builder.SetPhotoUrl(SelectedPackage.PhotoUrl);
                    builder.SetOldPrice(SelectedPackage.OldPrice);
                    builder.SetDetails(SelectedPackage.Details);
                    builder.SetGameId(GameProducts.GameId);
                    builder.SetOrderType(OrderType.Product);
                    if (await CheckPackageAccess(SelectedPackage.Id)) return;
                    break;
                default:
                    break;
            }
            var details = builder.Build();

            regionManager.RequestNavigate("LibraryContentRegion", "PurchaseControl");
            eventAggregator.GetEvent<SendEvent<PurchaseModel>>().Publish(details);
        }

        private async Task<bool> CheckPackageAccess(Guid Id)
        {
            try
            {
                EnabledProducts = false;
                var result = await gamesRepository.OwnsProduct(Id, GameModel.Id);
                EnabledProducts = true;
                if (result) dialogService.ShowDialog("InformationControl", new DialogParameters()
                        {
                            { "title", "Already own this product" },
                            { "message",  "You already own this product.\nCannot proceed." }
                        }, null);
                return result;
            }
            catch (Exception)
            {
                EnabledProducts = true;
                dialogService.ShowDialog("InformationControl", new DialogParameters()
                        {
                            { "title", "Error" },
                            { "message",  "Requested server is not reachable" }
                        }, null);
                return true;
            }
        }

        private void OpenCouponDialog()
        {
            dialogService.ShowDialog("ApplyCouponControl", new DialogParameters
            {
                { "CodeType", CodeTypes.Product }
            }, null);
        }

        private void Goto_NewsControl(NewsModel? news)
        {
            if (news is not null)
            {
                regionManager.RequestNavigate("GameDetailsContentRegion", "NewsControl");
                eventAggregator.GetEvent<SendEvent<NewsModel>>().Publish(news);
            }
        }

        private async void EventReceived(GameModel _gameModel)
        {
            try
            {
                ErrorText = string.Empty;
                this.GameModel = _gameModel;

                var getProducts = GetProducts();
                var getNews = GetNews();
                await Task.WhenAll(getProducts, getNews);
            }
            catch (Exception)
            {
                ErrorText = "Error while getting data";
            }
        }

        private async Task GetNews()
        {
            NewsList.Clear();
            var news = await gamesRepository.GetNews(GameModel.Id);
            if (news is not null)
            {
                foreach (var item in news) NewsList.Add(item);
            }
        }

        private async Task GetProducts()
        {
            EnabledProducts = true;
            MainButtonEnabled = true;

            GameProducts = await gamesRepository.GetProducts(GameModel.Id);
            if (!GameProducts.IsAvailable && !GameProducts.IsPurchased) MainButtonEnabled = false;
            if (!GameProducts.IsPurchased) EnabledProducts = false;
            if (GameProducts.GameId == Guid.Empty) throw new Exception();
        }
    }
}
