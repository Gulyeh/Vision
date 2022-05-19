using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Windows;
using VisionClient.Core.Events;
using VisionClient.Views;

namespace VisionClient.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        private int visionCoins = 0;
        public int VisionCoins
        {
            get { return visionCoins; }
            set { SetProperty(ref visionCoins, value); }
        }

        private Visibility borderVisibility = Visibility.Hidden;
        public Visibility BorderVisibility
        {
            get { return borderVisibility; }
            set { SetProperty(ref borderVisibility, value); }
        }

        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;

        public IRegionManager RegionManager { get; private set; }
        public DelegateCommand<string> NavigateCommand { get; set; }
        public DelegateCommand BuyMoreCommand { get; set; }
        public DelegateCommand UseCodeCommand { get; set; }

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IDialogService dialogService)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(HomeControl));
            NavigateCommand = new DelegateCommand<string>(Navigate);
            UseCodeCommand = new DelegateCommand(OpenCodeDialog);
            BuyMoreCommand = new DelegateCommand(BuyMoreDialog);
            eventAggregator.GetEvent<SendEvent<Visibility>>().Subscribe(x => BorderVisibility = x);
            RegionManager = regionManager;
        }

        private void Navigate(string uri)
        {
            regionManager.RequestNavigate("ContentRegion", uri);
        }

        private void OpenCodeDialog()
        {
            dialogService.ShowDialog("ApplyCouponControl");
        }

        private void BuyMoreDialog()
        {
            dialogService.ShowDialog("BuyMoreControl");
        }
    }
}