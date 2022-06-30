using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using System.Windows;
using VisionClient.Extensions;
using VisionClient.ViewModels;
using VisionClient.ViewModels.AdminPanelViewModels;
using VisionClient.Views;
using VisionClient.Views.AdminPanel.AddControls;
using VisionClient.Views.AdminPanel.ManageControls;
using VisionClient.Views.AdminPanel.UserControls;
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
            ViewModelLocationProvider.Register(typeof(ManagePaymentControl).ToString(), typeof(ManagePaymentControlViewModel));
            ViewModelLocationProvider.Register(typeof(ManageCouponsControl).ToString(), typeof(ManageCouponsControlViewModel));
            ViewModelLocationProvider.Register(typeof(EditCouponControl).ToString(), typeof(EditCouponControlViewModel));
            ViewModelLocationProvider.Register(typeof(ManageOrdersControl).ToString(), typeof(ManageOrdersControlViewModel));
            ViewModelLocationProvider.Register(typeof(ManageUsersControl).ToString(), typeof(ManageUsersControlViewModel));
            ViewModelLocationProvider.Register(typeof(EditUsersControl).ToString(), typeof(EditUsersControlViewModel));
            ViewModelLocationProvider.Register(typeof(ToggleBanAccessControl).ToString(), typeof(ToggleBanAccessControlViewModel));
            ViewModelLocationProvider.Register(typeof(ToggleBanGameControl).ToString(), typeof(ToggleBanGameControlViewModel));
            ViewModelLocationProvider.Register(typeof(KickControl).ToString(), typeof(KickControlViewModel));
            ViewModelLocationProvider.Register(typeof(UserUsedCodesControl).ToString(), typeof(UserUsedCodesControlViewModel));
            ViewModelLocationProvider.Register(typeof(GiveUserProductControl).ToString(), typeof(GiveUserProductControlViewModel));
            ViewModelLocationProvider.Register(typeof(ChangeUserRoleControl).ToString(), typeof(ChangeUserRoleControlViewModel));
            ViewModelLocationProvider.Register(typeof(TransactionsHistoryControl).ToString(), typeof(TransactionsHistoryControlViewModel));
            ViewModelLocationProvider.Register(typeof(ChangeUserCurrencyControl).ToString(), typeof(ChangeUserCurrencyControlViewModel));
        }
    }
}
