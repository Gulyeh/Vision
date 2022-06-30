using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
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
