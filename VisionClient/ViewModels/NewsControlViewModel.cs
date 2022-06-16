using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using VisionClient.Core.Events;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels
{
    internal class NewsControlViewModel : BindableBase
    {
        private NewsModel newsModel = new();
        public NewsModel NewsModel
        {
            get { return newsModel; }
            set { SetProperty(ref newsModel, value); }
        }

        private readonly IRegionManager regionManager;
        public DelegateCommand GoBackCommand { get; }

        public NewsControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            GoBackCommand = new DelegateCommand(GoBackward);
            eventAggregator.GetEvent<SendEvent<NewsModel>>().Subscribe(x => NewsModel = x);
        }

        private void GoBackward()
        {
            regionManager.RequestNavigate("GameDetailsContentRegion", "GameDetailsControl");
        }
    }
}
