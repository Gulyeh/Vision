using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisionClient.Core;
using VisionClient.Helpers;
using VisionClient.SignalR;

namespace VisionClient.Utility
{
    internal interface ILogoutClient
    {
        void Logout();
    }

    internal sealed class LogoutClient : ILogoutClient
    {
        private readonly IStaticData staticData;

        public LogoutClient(IStaticData staticData)
        {
            this.staticData = staticData;
        }

        public void Logout()
        {
            XMLCredentials.SaveCredentials("", "", false);
            staticData.ClearStatics();

            var bs = new LoginBootstrapper();
            bs.Run();

            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.Name.Equals("MainVisionWindow"));
            window?.Close();
        }
    }
}
