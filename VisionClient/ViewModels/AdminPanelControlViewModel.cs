using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace VisionClient.ViewModels
{
    internal class AdminPanelControlViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        public DelegateCommand BackwardCommand { get; }
        public DelegateCommand<string> AdminPanelContentCommand { get; }
        public AdminPanelControlViewModel(IRegionManager regionManager)
        {
            BackwardCommand = new DelegateCommand(NavigateToGames);
            AdminPanelContentCommand = new DelegateCommand<string>(SwitchContent);
            this.regionManager = regionManager;
        }

        private void SwitchContent(string name) => regionManager.RequestNavigate("AdminPanelRegion", name);
        
        private void NavigateToGames() => regionManager.RequestNavigate("LibraryContentRegion", "GamesControl");      
    }
}
