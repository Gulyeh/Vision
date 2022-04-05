using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.DbContexts;
using GamesDataService_API.Helpers;
using GamesDataService_API.Middleware;
using GamesDataService_API.Repository;
using GamesDataService_API.Repository.IRepository;
using GamesDataService_API.Services;
using GamesDataService_API.Services.IServices;
using HashidsNet;
using Microsoft.EntityFrameworkCore;

namespace GamesDataService_API.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config){
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("Connection"));
            });
            services.AddMemoryCache();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ErrorHandler>();
            services.AddScoped<IGamesRepository, GamesRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<ICacheService, CacheService>();
            return services;
        }
    }
}