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
using VisionClient.Views.AdminPanel.ManageControls;
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
            ViewModelLocationProvider.Register(typeof(AddNewsControl).ToString(), typeof(AddNewsControlViewModel));
            ViewModelLocationProvider.Register(typeof(AddCurrencyControl).ToString(), typeof(AddCurrencyControlViewModel));
            ViewModelLocationProvider.Register(typeof(AddGamePackageControl).ToString(), typeof(AddGamePackageControlViewModel));
            ViewModelLocationProvider.Register(typeof(AddPaymentControl).ToString(), typeof(AddPaymentControlViewModel));
            ViewModelLocationProvider.Register(typeof(AddCouponControl).ToString(), typeof(AddCouponControlViewModel));
            ViewModelLocationProvider.Register(typeof(ManageGamesControl).ToString(), typeof(ManageGamesControlViewModel));
            ViewModelLocationProvider.Register(typeof(ManageNewsControl).ToString(), typeof(ManageNewsControlViewModel));
            ViewModelLocationProvider.Register(typeof(EditNewsControl).ToString(), typeof(EditNewsControlViewModel));
            ViewModelLocationProvider.Register(typeof(ManageCurrencyControl).ToString(), typeof(ManageCurrencyControlViewModel));
            ViewModelLocationProvider.Register(typeof(EditCurrencyControl).ToString(), typeof(EditCurrencyControlViewModel));
            ViewModelLocationProvider.Register(typeof(ManageProductsControl).ToString(), typeof(ManageProductsControlViewModel));
            ViewModelLocationProvider.Register(typeof(EditPackageControl).ToString(), typeof(EditPackageControlViewModel));
        }
    }
}
