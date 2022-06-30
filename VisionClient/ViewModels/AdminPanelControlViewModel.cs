using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using VisionClient.Core;

namespace VisionClient.ViewModels
{
    internal class AdminPanelControlViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        public DelegateCommand BackwardCommand { get; }
        public DelegateCommand<string> AdminPanelContentCommand { get; }
        public IStaticData StaticData { get; }

        public AdminPanelControlViewModel(IRegionManager regionManager, IStaticData staticData)
        {
            BackwardCommand = new DelegateCommand(NavigateToGames);
            AdminPanelContentCommand = new DelegateCommand<string>(SwitchContent);
            this.regionManager = regionManager;
            StaticData = staticData;
        }

        private void SwitchContent(string name) => regionManager.RequestNavigate("AdminPanelRegion", name);

        private void NavigateToGames() => regionManager.RequestNavigate("LibraryContentRegion", "GamesControl");
    }
}
