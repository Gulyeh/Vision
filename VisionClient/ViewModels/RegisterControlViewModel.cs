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
    internal class RegisterControlViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        public DelegateCommand GoBackwardCommand { get; set; }
        public RegisterControlViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            GoBackwardCommand = new DelegateCommand(GoBackward);
        }

        private void GoBackward()
        {
            regionManager.RequestNavigate("LoginContent", "LoginControl");
        }
    }
}
