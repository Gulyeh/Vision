using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using VisionClient.Core.Events;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels
{
    internal class HomeControlViewModel : BindableBase
    {
        private HomeModel? leftPhoto;
        public HomeModel? LeftPhoto
        {
            get { return leftPhoto; }
            set
            {
                SetProperty(ref leftPhoto, value);
            }
        }

        private HomeModel? rightPhoto;
        public HomeModel? RightPhoto
        {
            get { return rightPhoto; }
            set
            {
                SetProperty(ref rightPhoto, value);
            }
        }

        private HomeModel? mainPhoto;
        public HomeModel? MainPhoto
        {
            get { return mainPhoto; }
            set
            {
                SetProperty(ref mainPhoto, value);
            }
        }

        private HomeModel? gameSelected;
        public HomeModel? GameSelected
        {
            get { return gameSelected; }
            set
            {
                SetProperty(ref gameSelected, value);
                ShowGameDetails(gameSelected);
            }
        }


        private DispatcherTimer dispatcherTimer;
        public ObservableCollection<HomeModel> GameList { get; set; }
        public DelegateCommand<string> ChangePhotoCommand { get; set; }
        public DelegateCommand<HomeModel> GetGameDetails { get; set; }
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private string nextButtonTick = "rightButton";

        public HomeControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            GameList = new ObservableCollection<HomeModel>();
            ChangePhotoCommand = new DelegateCommand<string>(SetMainPhoto);
            GetGameDetails = new DelegateCommand<HomeModel>(ShowGameDetails);
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            dispatcherTimer = new DispatcherTimer();
            SetupChangePhotoTimer();

            var model = new HomeModel()
            {
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas auctor, magna non ullamcorper porttitor, neque ligula condimentum justo, quis cursus mauris lorem vel est. Donec ut efficitur sapien. Sed eu nunc nisi. Duis sed sollicitudin turpis, at tempus urna. Quisque volutpat dapibus massa, ut faucibus ex venenatis quis. Maecenas ac dignissim elit, sed varius nisi. Donec tincidunt sapien nec ornare efficitur.",
                Title = "Swords of Legends Online",
                PhotoUrl = "https://www.allkeyshop.com/blog/wp-content/uploads/solo-banner.jpg",
                ProductInfo = new ProductInfoModel()
                {
                    Developer = "WanqShui IO",
                    Publisher = "Vision SA",
                    Genre = "Fantasy",
                    Language = "English"
                },
                Requirements = new RequirementsModel()
                {
                    CpuMinimum = "Intel 3999",
                    OsMinimum = "Windows 7",
                    GpuMinimum = "GTX 990",
                    MemoryMinimum = "4GB Memory",
                    StorageMinimum = "3GB Storage",
                    OsRecommended = "Windows 10",
                    CpuRecommended = "Intel i10 10000x",
                    GpuRecommended = "RTX 3090 Ti",
                    MemoryRecommended = "12GB Memory",
                    StorageRecommended = "3GB Storage"
                }
            };

            MainPhoto = model;
        }


        private void SetupChangePhotoTimer()
        {
            dispatcherTimer.Interval = TimeSpan.FromSeconds(30);
            dispatcherTimer.Tick += timer_ChangePhoto;
            dispatcherTimer.Start();
        }

        private void timer_ChangePhoto(object? sender, EventArgs e)
        {
            SetMainPhoto(nextButtonTick);
        }

        private void ShowGameDetails(HomeModel? game)
        {
            if (game is not null)
            {
                regionManager.RequestNavigate("ContentRegion", "HomeDetailsControl");
                eventAggregator.GetEvent<SendEvent<HomeModel>>().Publish(game);
            }
        }

        private void SetMainPhoto(string button)
        {
            HomeModel? temp;
            if (button == "leftButton")
            {
                temp = MainPhoto;
                MainPhoto = LeftPhoto;
                LeftPhoto = temp;
                nextButtonTick = "rightButton";
                return;
            }

            temp = MainPhoto;
            MainPhoto = RightPhoto;
            RightPhoto = temp;
            nextButtonTick = "leftButton";
        }
    }
}
