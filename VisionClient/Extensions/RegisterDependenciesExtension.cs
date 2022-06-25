using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;
using VisionClient.Core;
using VisionClient.Core.Repository;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services;
using VisionClient.Core.Services.IServices;
using VisionClient.SignalR;
using VisionClient.Utility;

namespace VisionClient.Extensions
{
    internal static class RegisterDependenciesExtension
    {
        internal static void RegisterLoginDependencies(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterServices(s =>
            {
                s.AddSingleton<IUsersService_Hubs, UsersService_Hubs>();
                s.AddSingleton<IStaticData, StaticData>();
                s.AddHttpClient<IAccountService, AccountService>();
                s.AddScoped<IAccountService, AccountService>();
                s.AddScoped<IAccountRepository, AccountRepository>();
            });
        }

        internal static void RegisterLoadingDependencies(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterServices(s =>
            {
                s.AddSingleton<IToastNotification, ToastNotification>();
                s.AddScoped<IGamesRepository, GamesRepository>();
                s.AddScoped<IGamesService, GamesService>();
            });
        }

        internal static void RegisterMainWindowDependencies(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterServices(s =>
            {
                s.AddSingleton<IOrderService_Hubs, OrderService_Hubs>();
                s.AddScoped<ICurrencyRepository, CurrencyRepository>();
                s.AddScoped<ICurrencyService, CurrencyService>();
                s.AddScoped<IPaymentService, PaymentService>();
                s.AddScoped<IPaymentRepository, PaymentRepository>();
                s.AddScoped<IUsersService, UsersService>();
                s.AddScoped<IUsersRepository, UsersRepository>();
                s.AddSingleton<IMessageService_Hubs, MessageService_Hubs>();
                s.AddScoped<ICouponService, CouponService>();
                s.AddScoped<ICouponRepository, CouponRepository>();
                s.AddScoped<IOrderService, OrderService>();
                s.AddScoped<IOrderRepository, OrderRepository>();
            });
        }
    }
}
