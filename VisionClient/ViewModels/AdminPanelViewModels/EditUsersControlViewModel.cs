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

        public EditUsersControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            BackwardCommand = new DelegateCommand(NavigateToUsers);
            this.regionManager = regionManager;

            eventAggregator.GetEvent<SendEvent<DetailedUserModel>>().Subscribe(x =>
            {
                UserModel = x;
            });
        }

        private void NavigateToUsers() => regionManager.RequestNavigate("AdminPanelRegion", "ManageUsersControl");
    }
}
