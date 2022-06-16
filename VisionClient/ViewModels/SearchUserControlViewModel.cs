using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using VisionClient.Core;

namespace VisionClient.ViewModels
{
    internal class SearchUserControlViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;


        public DelegateCommand<string> SwitchPanelCommand { get; }
        public DelegateCommand GoBackwardCommand { get; }
        public IStaticData StaticData { get; }

        public SearchUserControlViewModel(IRegionManager regionManager, IStaticData staticData)
        {
            regionManager.RegisterViewWithRegion("SearchUserContent", "SearchControl");
            GoBackwardCommand = new DelegateCommand(GoBackward);
            SwitchPanelCommand = new DelegateCommand<string>(SwitchPanel);
            this.regionManager = regionManager;
            StaticData = staticData;
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
