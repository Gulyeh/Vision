using Microsoft.EntityFrameworkCore;
using ProductsService_API.DbContexts;
using ProductsService_API.Helpers;
using ProductsService_API.Middleware;
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
            services.AddHttpClient<IGameDataService, GameDataService>();
            services.AddHttpClient<IGameAccessService, GameAccessService>();
            services.AddScoped<IGameAccessService, GameAccessService>();
            services.AddScoped<IGameDataService, GameDataService>();
            services.AddScoped<IGetCachedGames, GetCachedGames>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ErrorHandler>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IGamesRepository, GamesRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            return services;
        }
    }
}