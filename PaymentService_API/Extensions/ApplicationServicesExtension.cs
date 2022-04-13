using Microsoft.EntityFrameworkCore;
using PaymentService_API.DbContexts;
using PaymentService_API.Helpers;
using PaymentService_API.Middleware;
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
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ErrorHandler>();
            services.AddSingleton<IRabbitMQSender, RabbitMQMessageSender>();
            services.AddHostedService<RabbitMQOrderConsumer>();
            services.AddScoped<IStripeService, StripeService>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            return services;
        }
    }
}