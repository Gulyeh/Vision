using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UsersService_API.DbContexts;
using UsersService_API.Helpers;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services.IServices;
using UsersService_API.SignalR;

namespace UsersService_API.RabbitMQConsumer
{
    public class RabbitMQUnbanUserConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQUnbanUserConsumer> logger;
        private IConnection connection;
        private IModel channel;

        public RabbitMQUnbanUserConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory,
            ILogger<RabbitMQUnbanUserConsumer> logger)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password,
                Port = options.Value.Port
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "UnbanUserQueue", false, false, false, arguments: null);
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received message from queue: UnbanUserQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                Guid? userId = JsonConvert.DeserializeObject<Guid>(content);
                HandleMessage(userId).GetAwaiter().GetResult();
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("UnbanUserQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(Guid? userId)
        {
            if (userId is not null && userId != Guid.Empty)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);    
                if(user is not null){
                    user.IsBanned = false;
                    await db.SaveChangesAsync();
                }              
            }
        }
    }
}