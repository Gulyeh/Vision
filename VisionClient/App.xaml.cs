using Prism.Unity;
using System.Windows;

namespace VisionClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjM0NDE5QDMyMzAyZTMxMmUzMEpyRlJzZWM1T1BOa2NDam85ckx3UFFGZmlzQXBpeENzOHZ1NG4zRUJDVUE9");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            PrismContainerExtension.Init();
            base.OnStartup(e);
            var bs = new LoginBootstrapper();
            bs.Run();
        }
    }
}
