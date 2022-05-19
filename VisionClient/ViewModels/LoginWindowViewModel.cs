using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.ViewModels
{
    internal class LoginWindowViewModel : BindableBase
    {
        public LoginWindowViewModel(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion("LoginContent", "LoginControl");
        }
    }
}
