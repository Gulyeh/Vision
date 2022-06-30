using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UsersService_API.Helpers;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services.IServices;
using UsersService_API.SignalR;

namespace UsersService_API.RabbitMQConsumer
{
    public class RabbitMQBanUserConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQBanUserConsumer> logger;
        private IConnection connection;
        private IModel channel;

        public RabbitMQBanUserConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory,
            ILogger<RabbitMQBanUserConsumer> logger)
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
            channel.QueueDeclare(queue: "BanUserQueue", false, false, false, arguments: null);
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                try
                {
                    logger.LogInformation("Received message from queue: BanUserQueue");
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    Guid? userId = JsonConvert.DeserializeObject<Guid>(content);
                    HandleMessage(userId).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("BanUserQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(Guid? userId)
        {
            if (userId is not null && userId != Guid.Empty)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var usersHub = scope.ServiceProvider.GetRequiredService<IHubContext<UsersHub>>();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var isBanned = await userRepository.BanUser((Guid)userId);
                if (isBanned)
                {
                    var cachedIds = await cache.TryGetFromCache(HubTypes.Users);
                    if (cachedIds.Any(x => x.Key == userId)) await usersHub.Clients.Clients(cachedIds[(Guid)userId]).SendAsync("UserKicked", string.Empty);
                }
            }
        }
    }
}