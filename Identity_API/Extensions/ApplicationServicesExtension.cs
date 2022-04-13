using Identity_API.DbContexts;
using Identity_API.Helpers;
using Identity_API.Middleware;
using Identity_API.RabbitMQSender;
using Identity_API.Repository;
using Identity_API.Repository.IRepository;
using Identity_API.Services;
using Identity_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Identity_API.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("Connection"));
            });
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddScoped<IRabbitMQSender, RabbitMQMessageSender>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccessRepository, AccessRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ErrorHandler>();
            return services;
        }
    }
}