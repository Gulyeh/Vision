using Prism.Unity;
using System.Windows;

namespace VisionClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            PrismContainerExtension.Init();
            base.OnStartup(e);
            var bs = new LoginBootstrapper();
            bs.Run();
        }
    }
}
