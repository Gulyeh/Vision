using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService_API.Builders;
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
        private readonly ICacheService cacheService;
        private readonly IHubContext<OrderHub> hubContext;
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly ILogger<RabbitMQAccessConsumer> logger;

        public RabbitMQAccessConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory, 
            IRabbitMQSender rabbitMQSender, ILogger<RabbitMQAccessConsumer> logger)
        {
            using var scope = serviceScopeFactory.CreateScope();
            this.hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<OrderHub>>();
            this.cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
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
                var connIds = await cacheService.TryGetFromCache(data.UserId);
                if (connIds.Count() > 0) await hubContext.Clients.Clients(connIds).SendAsync("ProductPaymentDone", new { isSuccess = data.isSuccess, gameId = data.gameId, productId = data.productId });
                var emailGenerator = new EmailGenerator<UserAccessDto>(data);
                rabbitMQSender.SendMessage(emailGenerator.Generate(), "SendEmailQueue");
            }
        }
    }
}