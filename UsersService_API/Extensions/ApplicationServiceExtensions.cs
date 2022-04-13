using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using UsersService_API.DbContexts;
using UsersService_API.Dtos;
using UsersService_API.Entites;
using UsersService_API.Helpers;
using UsersService_API.Middleware;
using UsersService_API.RabbitMQConsumer;
using UsersService_API.RabbitMQSender;
using UsersService_API.Repository;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services;
using UsersService_API.Services.IServices;

namespace UsersService_API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config){
            services.AddDbContext<ApplicationDbContext>(opts => {
                opts.UseSqlServer(config.GetConnectionString("Connection"));
            });
            services.AddSignalR();
            services.AddMemoryCache();
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddHostedService<RabbitMQIdentityConsumer>();
            services.AddHostedService<RabbitMQAccessConsumer>();
            services.AddSingleton<IRabbitMQSender, RabbitMQMessageSender>();
            services.AddScoped<ICacheService, CacheService>();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IFriendsRepository, FriendsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ErrorHandler>();
            return services;
        }
    }
}