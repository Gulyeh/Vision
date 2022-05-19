using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System.Windows;
using VisionClient.Extensions;
using VisionClient.ViewModels;
using VisionClient.ViewModels.DialogsViewModels;
using VisionClient.Views;
using VisionClient.Views.Dialogs;
using VisionClient.Views.Login;
using VisionClient.Views.SettingsControls;

namespace VisionClient
{
    internal class LoadingBootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<LoadingWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
        }
    }
}
