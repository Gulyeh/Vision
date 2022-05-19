using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
