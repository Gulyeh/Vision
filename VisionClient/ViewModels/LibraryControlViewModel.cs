using Prism;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Linq;
using VisionClient.SignalR;
using VisionClient.Views;

namespace VisionClient.ViewModels
{
    internal class LibraryControlViewModel : BindableBase, IActiveAware
    {
        public event EventHandler? IsActiveChanged;

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                SetProperty(ref _isActive, value, RaiseIsActiveChanged);
                if (IsActive) CheckIfMessageContent();
            }
        }

        protected virtual void RaiseIsActiveChanged() => IsActiveChanged?.Invoke(this, EventArgs.Empty);
        
        private readonly IRegionManager regionManager;
        private readonly IMessageService_Hubs messageService_Hubs;

        public LibraryControlViewModel(IRegionManager regionManager, IMessageService_Hubs messageService_Hubs)
        {
            regionManager.RegisterViewWithRegion("LibraryContentRegion", typeof(GamesControl));
            regionManager.RegisterViewWithRegion("FriendsContentRegion", typeof(FriendsControl));
            this.regionManager = regionManager;
            this.messageService_Hubs = messageService_Hubs;
        }

        private async void CheckIfMessageContent()
        {
            var isMessageControl = regionManager.Regions["LibraryContentRegion"].ActiveViews.Any(v => v.GetType() == typeof(MessageControl));
            if (isMessageControl) await messageService_Hubs.CreateHubConnection(Guid.Empty);
        }
    }
}
