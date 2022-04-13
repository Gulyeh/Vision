using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameAccessService_API.DbContexts;
using GameAccessService_API.Helpers;
using GameAccessService_API.Middleware;
using GameAccessService_API.RabbitMQConsumer;
using GameAccessService_API.RabbitMQSender;
using GameAccessService_API.Repository;
using GameAccessService_API.Repository.IRepository;
using GameAccessService_API.Services;
using GameAccessService_API.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameAccessService_API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config){
            services.AddDbContext<ApplicationDbContext>(opts => {
                opts.UseSqlServer(config.GetConnectionString("Connection"));
            });          
            services.AddMemoryCache();
            services.AddScoped<ICacheService, CacheService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ErrorHandler>();
            services.AddScoped<IAccessRepository, AccessRepository>();
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddHostedService<RabbitMQOrderConsumer>();
            services.AddSingleton<IRabbitMQSender, RabbitMQMessageSender>();
            return services;
        }
    }
}