using Prism.Mvvm;
using Prism.Regions;
using VisionClient.Views;

namespace VisionClient.ViewModels
{
    internal class GamesControlViewModel : BindableBase
    {
        public GamesControlViewModel(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion("GameListContentRegion", typeof(GameListControl));
            regionManager.RegisterViewWithRegion("GameDetailsContentRegion", typeof(GameDetailsControl));
        }
    }
}
