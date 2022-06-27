using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Events;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class EditUsersControlViewModel : BindableBase
    {

        private DetailedUserModel userModel = new();
        public DetailedUserModel UserModel
        {
            get { return userModel; }
            set { SetProperty(ref userModel, value); }
        }

        private readonly IRegionManager regionManager;

        public DelegateCommand BackwardCommand { get; }
        public DelegateCommand<string> UserPanelContentCommand { get; }
        public EditUsersControlViewModel(IRegionManager regionManager)
        {
            BackwardCommand = new DelegateCommand(NavigateToUsers);
            UserPanelContentCommand = new DelegateCommand<string>(SwitchContent);
            this.regionManager = regionManager;
        }

        private void SwitchContent(string name) => regionManager.RequestNavigate("UserPanelRegion", name);
        private void NavigateToUsers() => regionManager.RequestNavigate("AdminPanelRegion", "ManageUsersControl");
    }
}
