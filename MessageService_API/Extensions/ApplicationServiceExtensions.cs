using MessageService_API.DbContexts;
using MessageService_API.Helpers;
using MessageService_API.Middleware;
using MessageService_API.RabbitMQConsumer;
using MessageService_API.Repository;
using MessageService_API.Repository.IRepository;
using MessageService_API.Services;
using MessageService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace MessagesService_API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(opts =>
            {
                opts.UseSqlServer(config.GetConnectionString("Connection"));
            });
            services.AddMemoryCache();
            services.AddSignalR(configure => { configure.MaximumReceiveMessageSize = null; });
            services.Configure<RabbitMQSettings>(config.GetSection("RabbitMQSettings"));
            services.AddHttpClient<IUsersService, UsersService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IConnectionsCacheService, ConnectionsCacheService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IChatCacheService, ChatCacheService>();
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHostedService<RabbitMQUsersConsumer>();
            services.AddScoped<ErrorHandler>();
            return services;
        }
    }
}