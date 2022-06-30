using CodesService_API.DbContexts;
using CodesService_API.Helpers;
using CodesService_API.Middleware;
using CodesService_API.Processor;
using CodesService_API.RabbitMQConsumer;
using CodesService_API.RabbitMQRPC;
using CodesService_API.RabbitMQSender;
using CodesService_API.Repository;
using CodesService_API.Repository.IRepository;
using CodesService_API.Services;
using CodesService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace CodesService_API.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("Connection"));
            });
            services.AddMemoryCache();
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ICodesRepository, CodesRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHostedService<RabbitMQCouponFailedConsumer>();
            services.AddHostedService<RabbitMQApplyCouponConsumer>();
            services.AddSingleton<IRabbitMQSender, RabbitMQMessageSender>();
            services.AddScoped<ErrorHandler>();
            services.AddSingleton<IRabbitMQRPC, RabbitMQRPCSender>();
            services.AddScoped<ICodeTypeProcessor, CodeTypeProcessor>();
            return services;
        }
    }
}