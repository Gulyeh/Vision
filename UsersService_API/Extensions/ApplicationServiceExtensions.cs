using Microsoft.EntityFrameworkCore;
using UsersService_API.DbContexts;
using UsersService_API.Helpers;
using UsersService_API.Middleware;
using UsersService_API.RabbitMQConsumer;
using UsersService_API.RabbitMQRPC;
using UsersService_API.RabbitMQSender;
using UsersService_API.Repository;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services;
using UsersService_API.Services.IServices;

namespace UsersService_API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(opts =>
            {
                opts.UseSqlServer(config.GetConnectionString("Connection"));
            });
            services.AddSignalR();
            services.AddMemoryCache();
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddHostedService<RabbitMQIdentityConsumer>();
            services.AddHostedService<RabbitMQCurrencyConsumer>();
            services.AddHostedService<RabbitMQCouponAccessConsumer>();
            services.AddHostedService<RabbitMQBanUserConsumer>();
            services.AddHostedService<RabbitMQUnbanUserConsumer>();
            services.AddHostedService<RabbitMQDeleteUserConsumer>();
            services.AddHostedService<RabbitMQKickUserConsumer>();
            services.AddHostedService<RabbitMQUserExistsConsumer>();
            services.AddHostedService<RabbitMQIsUserBlockedConsumer>();
            services.AddHostedService<RabbitMQMessageNotificationConsumer>();
            services.AddSingleton<IRabbitMQSender, RabbitMQMessageSender>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddSingleton<IRabbitMQRPC, RabbitMQRPCSender>();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IFriendsRepository, FriendsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ErrorHandler>();

            return services;
        }
    }
}