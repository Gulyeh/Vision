using Prism.Events;
using System.Windows.Controls;
using VisionClient.Core;
using VisionClient.Helpers;
using VisionClient.SignalR;
using VisionClient.Utility;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class LogoutControlViewModel : DialogHelper
    {
        private readonly IUsersService_Hubs usersService_Hubs;

        public LogoutControlViewModel(IEventAggregator eventAggregator, IUsersService_Hubs usersService_Hubs) : base(eventAggregator)
        {
            this.usersService_Hubs = usersService_Hubs;
        }

        protected override void Execute(object? data) => usersService_Hubs.Disconnect();
    }
}
