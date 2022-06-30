using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using VisionClient.Core.Events;
using VisionClient.Core.Helpers.Aggregators;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels
{
    internal class HomeDetailsControlViewModel : BindableBase
    {
        private GameModel gameDetails = new();
        public GameModel GameDetails
        {
            get { return gameDetails; }
            set { SetProperty(ref gameDetails, value); }
        }

        private readonly IRegionManager regionManager;
        public DelegateCommand GoHomeCommand { get; }

        public HomeDetailsControlViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            eventAggregator.GetEvent<SendEvent<HomeToDetails>>().Subscribe(x => GameDetails = x.Game);
            GoHomeCommand = new DelegateCommand(NavigateToHome);
            this.regionManager = regionManager;
        }

        private void NavigateToHome()
        {
            regionManager.RequestNavigate("ContentRegion", "HomeControl");
        }
    }
}
