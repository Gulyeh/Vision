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
        private readonly IStaticData StaticData;

        public LogoutControlViewModel(IEventAggregator eventAggregator,
            IUsersService_Hubs usersService_Hubs, IStaticData staticData) : base(eventAggregator)
        {
            this.usersService_Hubs = usersService_Hubs;
            this.StaticData = staticData;
        }

        protected async override void Execute(object? window)
        {
            XMLCredentials.SaveCredentials("", "", false);
            await usersService_Hubs.Disconnect();
            StaticData.ClearStatics();

            var bs = new LoginBootstrapper();
            bs.Run();

            CloseParentWindowHelper.Close(window as UserControl);
        }
    }
}
