using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.ViewModels
{
    internal class AdminPanelControlViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        public DelegateCommand BackwardCommand { get; set; }
        public DelegateCommand<string> AdminPanelContentCommand { get; set; }
        public AdminPanelControlViewModel(IRegionManager regionManager)
        {
            BackwardCommand = new DelegateCommand(navigateToGames);
            AdminPanelContentCommand = new DelegateCommand<string>(SwitchContent);
            this.regionManager = regionManager;
        }

        private void SwitchContent(string name)
        {
            regionManager.RequestNavigate("AdminPanelRegion", name);
        }

        private void navigateToGames()
        {
            regionManager.RequestNavigate("LibraryContentRegion", "GamesControl");
        }
    }
}
