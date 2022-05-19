using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.ViewModels
{
    internal class SearchUserControlViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        public DelegateCommand<string> SwitchPanelCommand { get; set; }
        public DelegateCommand GoBackwardCommand { get; set; }

        public SearchUserControlViewModel(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion("SearchUserContent", "SearchControl");
            GoBackwardCommand = new DelegateCommand(GoBackward);
            SwitchPanelCommand = new DelegateCommand<string>(SwitchPanel);
            this.regionManager = regionManager;
        }

        private void SwitchPanel(string name)
        {
            regionManager.RequestNavigate("SearchUserContent", name);
        }

        private void GoBackward()
        {
            regionManager.RequestNavigate("LibraryContentRegion", "GamesControl");
        }
    }
}
