using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VisionClient.Utility
{
    internal static class CheckUpdate
    {
        public static void CheckTimer()
        {
            AutoUpdater.RunUpdateAsAdmin = true;
            AutoUpdater.UpdateFormSize = new System.Drawing.Size(400, 900);
            AutoUpdater.ShowSkipButton = false;

            AutoUpdater.Start("https://gist.githubusercontent.com/Gulyeh/be47e02d0950aed152385f5b8b3fe5ce/raw/58f6331708970dbe14eb4b9c7ba9a211d23999b9/UpdateVersion.xml");

            DispatcherTimer timer = new() { Interval = TimeSpan.FromMinutes(10) };
            timer.Tick += delegate
            {
                AutoUpdater.Start("https://gist.githubusercontent.com/Gulyeh/be47e02d0950aed152385f5b8b3fe5ce/raw/58f6331708970dbe14eb4b9c7ba9a211d23999b9/UpdateVersion.xml");
            };
            timer.Start();
        }
    }
}
