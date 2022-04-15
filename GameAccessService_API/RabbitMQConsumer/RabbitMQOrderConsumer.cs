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
        private readonly IAccessRepository accessRepository;
        private readonly IRabbitMQSender rabbitMQSender;
        private IConnection connection;
        private IModel channel;

        public RabbitMQOrderConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory scopeFactory, IRabbitMQSender rabbitMQSender)
        {
            using var scope = scopeFactory.CreateScope();
            accessRepository = scope.ServiceProvider.GetRequiredService<IAccessRepository>();
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
                if (data.productId is not null)
                {
                    data.isSuccess = await accessRepository.AddProductOrGame(data.userId, data.gameId, (Guid)data.productId);
                }
                else
                {
                    data.isSuccess = await accessRepository.AddProductOrGame(data.userId, data.gameId);
                }

                rabbitMQSender.SendMessage(data, "ProductAccessDoneQueue");
                rabbitMQSender.SendMessage(data, "AccessProductQueue");
            }
        }
    }
}