using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using VisionClient.Core;
using VisionClient.Core.Events;
using VisionClient.Core.Helpers.Aggregators;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels
{
    internal class HomeControlViewModel : BindableBase
    {
        private GameModel leftPhoto = new();
        public GameModel LeftPhoto
        {
            get { return leftPhoto; }
            set
            {
                SetProperty(ref leftPhoto, value);
            }
        }

        private GameModel rightPhoto = new();
        public GameModel RightPhoto
        {
            get { return rightPhoto; }
            set
            {
                SetProperty(ref rightPhoto, value);
            }
        }

        private GameModel mainPhoto = new();
        public GameModel MainPhoto
        {
            get { return mainPhoto; }
            set
            {
                SetProperty(ref mainPhoto, value);
            }
        }

        private GameModel? gameSelected;
        public GameModel? GameSelected
        {
            get { return gameSelected; }
            set
            {
                SetProperty(ref gameSelected, value);
                ShowGameDetails(gameSelected);
            }
        }


        private readonly DispatcherTimer dispatcherTimer;
        public ObservableCollection<GameModel> GameList { get; }
        public DelegateCommand<string> ChangePhotoCommand { get; }
        public DelegateCommand<GameModel> GetGameDetails { get; }
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private string nextButtonTick = "rightButton";

        public HomeControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IStaticData StaticData)
        {
            GameList = StaticData.GameModels;
            ChangePhotoCommand = new DelegateCommand<string>(SetMainPhoto);
            GetGameDetails = new DelegateCommand<GameModel>(ShowGameDetails);
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            dispatcherTimer = new DispatcherTimer();
            SetupChangePhotoTimer();
            SetPhotos();
        }

        private void SetPhotos()
        {
            if (GameList.Count > 0) MainPhoto = GameList[0];
            if (GameList.Count > 1) LeftPhoto = GameList[1];
            if (GameList.Count > 2) RightPhoto = GameList[2];
        }

        private void SetupChangePhotoTimer()
        {
            dispatcherTimer.Interval = TimeSpan.FromSeconds(30);
            dispatcherTimer.Tick += Timer_ChangePhoto;
            dispatcherTimer.Start();
        }

        private void Timer_ChangePhoto(object? sender, EventArgs e)
        {
            SetMainPhoto(nextButtonTick);
        }

        private void ShowGameDetails(GameModel? game)
        {
            if (game is not null && game.Id != Guid.Empty)
            {
                regionManager.RequestNavigate("ContentRegion", "HomeDetailsControl");
                eventAggregator.GetEvent<SendEvent<HomeToDetails>>().Publish(new HomeToDetails(game));
            }
        }

        private void SetMainPhoto(string button)
        {
            GameModel? temp;
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
