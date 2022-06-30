using Microsoft.EntityFrameworkCore;
using ProdcutsService_API.RabbitMQConsumer;
using ProdcutsService_API.RabbitMQRPC;
using ProdcutsService_API.RabbitMQSender;
using ProductsService_API.DbContexts;
using ProductsService_API.Helpers;
using ProductsService_API.Middleware;
using ProductsService_API.RabbitMQConsumer;
using ProductsService_API.Repository;
using ProductsService_API.Repository.IRepository;
using ProductsService_API.Services;
using ProductsService_API.Services.IServices;

namespace ProductsService_API.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("Connection"));
            });
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddMemoryCache();
            services.AddHostedService<RabbitMQNewGameConsumer>();
            services.AddHostedService<RabbitMQDeleteGameConsumer>();
            services.AddHostedService<RabbitMQEditGameConsumer>();
            services.AddHostedService<RabbitMQGetProductsConsumer>();
            services.AddHostedService<RabbitMQCheckProductExistsConsumer>();
            services.AddSingleton<IRabbitMQSender, RabbitMQMessageSender>();
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddScoped<IGetCachedGames, GetCachedGames>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ErrorHandler>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddSingleton<IRabbitMQRPC, RabbitMQRPCSender>();
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IGamesRepository, GamesRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            return services;
        }
    }
}