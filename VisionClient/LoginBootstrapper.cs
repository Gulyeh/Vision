using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Net.Http;
using System.Windows;
using VisionClient.Core.Repository;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services;
using VisionClient.Core.Services.IServices;
using VisionClient.Extensions;
using VisionClient.ViewModels;
using VisionClient.ViewModels.DialogsViewModels;
using VisionClient.Views;
using VisionClient.Views.Dialogs;
using VisionClient.Views.Login;
using VisionClient.Views.Login.Dialogs;
using VisionClient.Views.SettingsControls;

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
            RegisterLoginDependenciesExtension.RegisterLoginDependencies(containerRegistry);
            containerRegistry.RegisterForNavigation<LoginControl>("LoginControl");
            containerRegistry.RegisterForNavigation<RegisterControl>("RegisterControl");
            containerRegistry.RegisterDialog<TFAControl, TFAControlViewModel>();
            containerRegistry.RegisterDialog<UserBannedControl, UserBannedControlViewModel>();
            containerRegistry.RegisterDialog<InformationControl, InformationControlViewModel>();
            containerRegistry.RegisterDialog<ForgotPasswordControl, ForgotPasswordControlViewModel>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            ViewModelLocationProvider.Register(typeof(LoginControl).ToString(), typeof(LoginControlViewModel));
            ViewModelLocationProvider.Register(typeof(RegisterControl).ToString(), typeof(RegisterControlViewModel));
        }

    }
}
