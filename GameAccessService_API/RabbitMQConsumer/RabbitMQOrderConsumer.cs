using GameAccessService_API.Helpers;
using GameAccessService_API.Messages;
using GameAccessService_API.RabbitMQSender;
using GameAccessService_API.Repository.IRepository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace GameAccessService_API.RabbitMQConsumer
{
    public class RabbitMQOrderConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMQOrderConsumer> logger;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IRabbitMQSender rabbitMQSender;
        private IConnection connection;
        private IModel channel;

        public RabbitMQOrderConsumer(IOptions<RabbitMQSettings> options, ILogger<RabbitMQOrderConsumer> logger, IServiceScopeFactory scopeFactory, IRabbitMQSender rabbitMQSender)
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;
            this.rabbitMQSender = rabbitMQSender;

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
                logger.LogInformation("RabbitMQ Consumed request from queue: AccessProductQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                UserPurchaseDto? gameData = JsonConvert.DeserializeObject<UserPurchaseDto>(content);
                HandleMessage(gameData).GetAwaiter().GetResult();

                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("AccessProductQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UserPurchaseDto? data)
        {
            if (data is not null)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var accessRepository = scope.ServiceProvider.GetRequiredService<IAccessRepository>();
                    data.IsSuccess = await accessRepository.AddProductOrGame(data.UserId, data.GameId, data.ProductId);
                    rabbitMQSender.SendMessage(data, "ProductAccessDoneQueue");
                    scope.Dispose();
                }
                catch (Exception)
                {
                    data.IsSuccess = false;
                    rabbitMQSender.SendMessage(data, "ProductAccessDoneQueue");
                }
            }
        }
    }
}