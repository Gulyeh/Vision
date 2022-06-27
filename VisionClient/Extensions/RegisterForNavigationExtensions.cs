using Prism.Ioc;
using VisionClient.Views;
using VisionClient.Views.AdminPanel.AddControls;
using VisionClient.Views.AdminPanel.ManageControls;
using VisionClient.Views.AdminPanel.UserControls;
using VisionClient.Views.SettingsControls;

namespace VisionClient.Extensions
{
    internal static class RegisterForNavigationExtensions
    {
        public static void RegisterViews(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LibraryControl>("LibraryControl");
            containerRegistry.RegisterForNavigation<FriendsControl>("FriendsControl");
            containerRegistry.RegisterForNavigation<GameListControl>("GameListControl");
            containerRegistry.RegisterForNavigation<GameDetailsControl>("GameDetailsControl");
            containerRegistry.RegisterForNavigation<HomeControl>("HomeControl");
            containerRegistry.RegisterForNavigation<HomeDetailsControl>("HomeDetailsControl");
            containerRegistry.RegisterForNavigation<NewsControl>("NewsControl");
            containerRegistry.RegisterForNavigation<SettingsControl>("SettingsControl");
            containerRegistry.RegisterForNavigation<MessageControl>("MessageControl");
            containerRegistry.RegisterForNavigation<GamesControl>("GamesControl");
            containerRegistry.RegisterForNavigation<ProfileControl>("ProfileControl");
            containerRegistry.RegisterForNavigation<SecurityControl>("SecurityControl");
            containerRegistry.RegisterForNavigation<AuthenticationControl>("AuthenticationControl");
            containerRegistry.RegisterForNavigation<AdminPanelControl>("AdminPanelControl");
            containerRegistry.RegisterForNavigation<SearchUserControl>("SearchUserControl");
            containerRegistry.RegisterForNavigation<SearchControl>("SearchControl");
            containerRegistry.RegisterForNavigation<PendingControl>("PendingControl");
            containerRegistry.RegisterForNavigation<RequestsControl>("RequestsControl");
            containerRegistry.RegisterForNavigation<PurchaseControl>("PurchaseControl");
            containerRegistry.RegisterForNavigation<AddGameControl>("AddGameControl");
            containerRegistry.RegisterForNavigation<AddNewsControl>("AddNewsControl");
            containerRegistry.RegisterForNavigation<AddCurrencyControl>("AddCurrencyControl");
            containerRegistry.RegisterForNavigation<AddGamePackageControl>("AddGamePackageControl");
            containerRegistry.RegisterForNavigation<AddPaymentControl>("AddPaymentControl");
            containerRegistry.RegisterForNavigation<AddCouponControl>("AddCouponControl");
            containerRegistry.RegisterForNavigation<ManageGamesControl>("ManageGamesControl");
            containerRegistry.RegisterForNavigation<ManageNewsControl>("ManageNewsControl");
            containerRegistry.RegisterForNavigation<EditNewsControl>("EditNewsControl");
            containerRegistry.RegisterForNavigation<ManageCurrencyControl>("ManageCurrencyControl");
            containerRegistry.RegisterForNavigation<EditCurrencyControl>("EditCurrencyControl");
            containerRegistry.RegisterForNavigation<ManageProductsControl>("ManageProductsControl");
            containerRegistry.RegisterForNavigation<EditPackageControl>("EditPackageControl");
            containerRegistry.RegisterForNavigation<ManagePaymentControl>("ManagePaymentControl");
            containerRegistry.RegisterForNavigation<ManageCouponsControl>("ManageCouponsControl");
            containerRegistry.RegisterForNavigation<EditCouponControl>("EditCouponControl");
            containerRegistry.RegisterForNavigation<ManageOrdersControl>("ManageOrdersControl");
            containerRegistry.RegisterForNavigation<ManageUsersControl>("ManageUsersControl");
            containerRegistry.RegisterForNavigation<EditUsersControl>("EditUsersControl");
            containerRegistry.RegisterForNavigation<ToggleBanAccessControl>("ToggleBanAccessControl");
            containerRegistry.RegisterForNavigation<ToggleBanGameControl>("ToggleBanGameControl");
            containerRegistry.RegisterForNavigation<UserUsedCodesControl>("UserUsedCodesControl");
            containerRegistry.RegisterForNavigation<GiveUserProductControl>("GiveUserProductControl");
            containerRegistry.RegisterForNavigation<ChangeUserRoleControl>("ChangeUserRoleControl");
            containerRegistry.RegisterForNavigation<KickControl>("KickControl");
            containerRegistry.RegisterForNavigation<TransactionsHistoryControl>("TransactionsHistoryControl");
        }
    }
}
