using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UsersService_API.Helpers;
using UsersService_API.Messages;
using UsersService_API.Services.IServices;
using UsersService_API.SignalR;

namespace UsersService_API.RabbitMQConsumer
{
    public class RabbitMQAccessConsumer : BackgroundService
    {
        private readonly ICacheService cacheService;
        private readonly IHubContext<UsersHub> hubContext;
        private IConnection connection;
        private IModel channel;

        public RabbitMQAccessConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory)
        {
            using var scope = serviceScopeFactory.CreateScope();
            this.cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
            this.hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<UsersHub>>();

            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "AccessProductQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                UserGameDto? gameData = JsonConvert.DeserializeObject<UserGameDto>(content);
                HandleMessage(gameData).GetAwaiter().GetResult();

                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("AccessProductQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UserGameDto? data)
        {
            if (data is not null)
            {
                var userCache = await cacheService.TryGetFromCache();
                if (userCache.ContainsKey(data.userId))
                {
                    var connIds = userCache.GetValueOrDefault(data.userId);
                    if (connIds is not null)
                    {
                        await hubContext.Clients.Clients(connIds).SendAsync("GamePurchased", new GamePurchased { gameId = data.gameId, productId = data.productId });
                    }
                }
            }
        }
    }
}