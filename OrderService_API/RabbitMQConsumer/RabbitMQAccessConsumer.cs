using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.RabbitMQSender;
using OrderService_API.Services.IServices;
using OrderService_API.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace OrderService_API.RabbitMQConsumer
{
    public class RabbitMQAccessConsumer : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly ILogger<RabbitMQAccessConsumer> logger;

        public RabbitMQAccessConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory,
            IRabbitMQSender rabbitMQSender, ILogger<RabbitMQAccessConsumer> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "ProductAccessDoneQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received data from queue: ProductAccessDoneQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                UserAccessDto? access = JsonConvert.DeserializeObject<UserAccessDto>(content);
                HandleMessage(access).GetAwaiter().GetResult();
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("ProductAccessDoneQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UserAccessDto? data)
        {
            if (data is not null)
            {
                List<string> connIds = new();
                using var scope = serviceScopeFactory.CreateScope();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<OrderHub>>();
                try
                {
                    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                    connIds = await cacheService.TryGetFromCache(data.UserId);
                    if (connIds.Count() > 0) await hubContext.Clients.Clients(connIds).SendAsync("ProductPaymentDone", data.IsSuccess);
                    scope.Dispose();
                }
                catch (Exception)
                {
                    if (connIds.Count() > 0) await hubContext.Clients.Clients(connIds).SendAsync("ProductPaymentDone", data.IsSuccess);
                }
            }
        }
    }
}