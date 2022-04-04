using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.DbContexts;
using CodesService_API.Middleware;
using CodesService_API.Repository;
using CodesService_API.Repository.IRepository;
using HashidsNet;
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
            services.AddScoped<ICodesRepository, CodesRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ErrorHandler>();
            return services;
        }
    }
}