using Microsoft.EntityFrameworkCore;
using OrderService_API.DbContexts;
using OrderService_API.Helpers;
using OrderService_API.Middleware;
using OrderService_API.RabbitMQConsumer;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository;
using OrderService_API.Repository.IRepository;
using OrderService_API.Services;
using OrderService_API.Services.IServices;

namespace OrderService_API.Extensions
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
            services.AddSignalR();
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ErrorHandler>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddHttpClient<IProductsService, ProductsService>();
            services.AddScoped<IProductsService, ProductsService>();
            services.AddHostedService<RabbitMQPaymentConsumer>();
            services.AddHostedService<RabbitMQPaymentCompletedConsumer>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddSingleton<IRabbitMQSender, RabbitMQMessageSender>();
            return services;
        }
    }
}