using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Events;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels
{
    internal class HomeDetailsControlViewModel : BindableBase
    {
        private HomeModel gameDetails = new();
        public HomeModel GameDetails
        {
            get { return gameDetails; }
            set { SetProperty(ref gameDetails, value); }
        }

        private readonly IRegionManager regionManager;
        public DelegateCommand GoHomeCommand { get; set; }

        public HomeDetailsControlViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            eventAggregator.GetEvent<SendEvent<HomeModel>>().Subscribe(x => GameDetails = x);
            GoHomeCommand = new DelegateCommand(navigateToHome);
            this.regionManager = regionManager;
        }

        private void navigateToHome()
        {
            regionManager.RequestNavigate("ContentRegion", "HomeControl");
        }
    }
}
