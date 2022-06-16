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
    public class RabbitMQCouponAccessConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQCouponAccessConsumer> logger;
        private IConnection connection;
        private IModel channel;

        public RabbitMQCouponAccessConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory,
            ILogger<RabbitMQCouponAccessConsumer> logger)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "AccessCouponProductQueue", false, false, false, arguments: null);
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received message from queue: AccessCouponProductQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                UserGameDto? gameData = JsonConvert.DeserializeObject<UserGameDto>(content);
                HandleMessage(gameData).GetAwaiter().GetResult();

                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("AccessCouponProductQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UserGameDto? data)
        {
            if (data is not null)
            {
                List<string>? connIds = null;
                using var scope = serviceScopeFactory.CreateScope();
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<UsersHub>>();
                try
                {
                    var userCache = await cacheService.TryGetFromCache(HubTypes.Users);
                    if (userCache.ContainsKey(data.UserId))
                    {
                        connIds = userCache.GetValueOrDefault(data.UserId);
                        if (connIds is not null) await hubContext.Clients.Clients(connIds).SendAsync("ProductCodeUsed", new GamePurchased(data.IsSuccess, data.GameId, data.ProductId));
                    }
                }
                catch (Exception)
                {
                    var product = data.ProductId == Guid.Empty ? "Game" : "Package";
                    if (connIds is not null && connIds.Count > 0) await hubContext.Clients.Clients(connIds).SendAsync("CodeFailed", new[] { $"{product} coupon could not be applied correctly" });
                }
            }
        }
    }
}