using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using VisionClient.Core.Events;
using VisionClient.Views;

namespace VisionClient.ViewModels
{
    internal class LibraryControlViewModel : BindableBase
    {
        public LibraryControlViewModel(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion("LibraryContentRegion", typeof(GamesControl));
            regionManager.RegisterViewWithRegion("FriendsContentRegion", typeof(FriendsControl));
        }
    }
}
