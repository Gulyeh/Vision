using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System.Windows;
using VisionClient.Extensions;
using VisionClient.SignalR;
using VisionClient.ViewModels;
using VisionClient.ViewModels.AdminPanelViewModels;
using VisionClient.Views;
using VisionClient.Views.AdminPanel.AddControls;
using VisionClient.Views.SettingsControls;

namespace VisionClient
{
    internal class Bootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterForNavigationExtensions.RegisterViews(containerRegistry);
            RegisterDialogExtensions.RegisterDialogs(containerRegistry);
            RegisterDependenciesExtension.RegisterMainWindowDependencies(containerRegistry);
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register(typeof(ProfileControl).ToString(), typeof(ProfileControlViewModel));
            ViewModelLocationProvider.Register(typeof(SecurityControl).ToString(), typeof(SecurityControlViewModel));
            ViewModelLocationProvider.Register(typeof(AuthenticationControl).ToString(), typeof(AuthenticationControlViewModel));
            ViewModelLocationProvider.Register(typeof(AddGameControl).ToString(), typeof(AddGameControlViewModel));
        }
    }
}
