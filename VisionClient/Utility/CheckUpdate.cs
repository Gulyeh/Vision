using AutoUpdaterDotNET;
using System;
using System.Windows.Threading;
using VisionClient.Core;

namespace VisionClient.Utility
{
    internal static class CheckUpdate
    {
        public static void CheckTimer()
        {
            AutoUpdater.RunUpdateAsAdmin = true;
            AutoUpdater.AppTitle = "Vision";
            AutoUpdater.ShowSkipButton = false;

            AutoUpdater.Start(ConnectionData.UpdateData);

            DispatcherTimer timer = new() { Interval = TimeSpan.FromMinutes(10) };
            timer.Tick += delegate
            {
                AutoUpdater.Start(ConnectionData.UpdateData);
            };
            timer.Start();
        }
    }
}
