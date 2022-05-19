using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using VisionClient.Core.Events;
using VisionClient.Core.Helpers.Aggregators;
using VisionClient.Core.Models;

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

        private ProductsModel? selectedPackage;
        public ProductsModel? SelectedPackage
        {
            get { return selectedPackage; }
            set 
            { 
                if(value is not null)
                {
                    SetProperty(ref selectedPackage, value);
                    NavigateToPurchase("PackageButton");
                }
            }
        }

        private string? mainButton;
        public string? MainButton
        {
            get { return mainButton; }
            set { SetProperty(ref mainButton, value); }
        }

        private NewsModel? selectedNews;
        public NewsModel? SelectedNews
        {
            get { return selectedNews; }
            set 
            { 
                SetProperty(ref selectedNews, value);
                goTo_NewsControl(selectedNews);
            }
        }

        public ObservableCollection<NewsModel> newsList { get; set; }
        public ObservableCollection<ProductsModel> productList { get; set; }
        public DelegateCommand MainButtonCommand { get; set; }
        public DelegateCommand OpenCouponDialogCommand { get; set; }
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;

        public GameDetailsControlViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IDialogService dialogService)
        {
            newsList = new ObservableCollection<NewsModel>();     
            productList = new ObservableCollection<ProductsModel>();
            OpenCouponDialogCommand = new DelegateCommand(OpenCouponDialog);
            MainButtonCommand = new DelegateCommand(MainButtonPressed);
            eventAggregator.GetEvent<SendEvent<GameModel>>().Subscribe(EventReceived);
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.dialogService = dialogService;
        }

        private void MainButtonPressed()
        {
            switch (GameModel.IsPurchased)
            {
                case true:
                    break;
                default:
                    NavigateToPurchase("MainButton");
                    break;
            }
        }

        private void NavigateToPurchase(string data)
        {
            var details = new GameDetailsToPurchase();
            switch (data)
            {
                case "MainButton":
                    details.Id = GameModel.Id;
                    break;
                case "PackageButton":
                    if(SelectedPackage is not null) details.Id = SelectedPackage.Id;
                    break;
                default:
                    break;
            }

            regionManager.RequestNavigate("LibraryContentRegion", "PurchaseControl");
            eventAggregator.GetEvent<SendEvent<GameDetailsToPurchase>>().Publish(details);
        }

        private void OpenCouponDialog()
        {
            dialogService.ShowDialog("ApplyCouponControl");
        }

        private void goTo_NewsControl(NewsModel? news)
        {
            if(news is not null)
            {
                regionManager.RequestNavigate("GameDetailsContentRegion", "NewsControl");
                eventAggregator.GetEvent<SendEvent<NewsModel>>().Publish(news);
            }
        }

        private void EventReceived(GameModel _gameModel)
        {
            this.GameModel = _gameModel;

            if (_gameModel.News is not null)
            {
                foreach(var news in _gameModel.News)
                {
                    newsList.Add(news);
                }
            }

            if (_gameModel.Products is not null)
            {
                foreach(var product in _gameModel.Products)
                {
                    productList.Add(product);
                }
            }
        }
    }
}
