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
using VisionClient.Views.Login.Dialogs;

namespace VisionClient
{
    internal class LoginBootstrapper : PrismBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<LoginWindow>();
        }
        protected override IContainerExtension CreateContainerExtension() => PrismContainerExtension.Current;

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterDependenciesExtension.RegisterLoginDependencies(containerRegistry);
            containerRegistry.RegisterForNavigation<LoginControl>("LoginControl");
            containerRegistry.RegisterForNavigation<RegisterControl>("RegisterControl");
            containerRegistry.RegisterDialog<TFAControl, TFAControlViewModel>();
            containerRegistry.RegisterDialog<UserBannedControl, UserBannedControlViewModel>();
            containerRegistry.RegisterDialog<InformationControl, InformationControlViewModel>();
            containerRegistry.RegisterDialog<ForgotPasswordControl, ForgotPasswordControlViewModel>();
            containerRegistry.RegisterDialog<ResendEmailControl, ResendEmailControlViewModel>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register(typeof(LoginControl).ToString(), typeof(LoginControlViewModel));
            ViewModelLocationProvider.Register(typeof(RegisterControl).ToString(), typeof(RegisterControlViewModel));
        }

    }
}
