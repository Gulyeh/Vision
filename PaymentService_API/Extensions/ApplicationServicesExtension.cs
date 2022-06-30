using Microsoft.EntityFrameworkCore;
using PaymentService_API.DbContexts;
using PaymentService_API.Helpers;
using PaymentService_API.Middleware;
using PaymentService_API.Processors;
using PaymentService_API.Processors.Interfaces;
using PaymentService_API.RabbitMQConsumer;
using PaymentService_API.RabbitMQSender;
using PaymentService_API.Repository;
using PaymentService_API.Repository.IRepository;
using PaymentService_API.Services;
using PaymentService_API.Services.IServices;

namespace PaymentService_API.Extensions
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
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ErrorHandler>();
            services.AddSingleton<IRabbitMQSender, RabbitMQMessageSender>();
            services.AddHostedService<RabbitMQOrderConsumer>();
            services.AddScoped<IStripeService, StripeService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IPaymentProcessor, PaymentUrlProcessor>();
            return services;
        }
    }
}