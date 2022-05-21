﻿using Prism.Ioc;
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
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register(typeof(ProfileControl).ToString(), typeof(ProfileControlViewModel));
            ViewModelLocationProvider.Register(typeof(SecurityControl).ToString(), typeof(SecurityControlViewModel));
            ViewModelLocationProvider.Register(typeof(AuthenticationControl).ToString(), typeof(AuthenticationControlViewModel));
        }
    }
}