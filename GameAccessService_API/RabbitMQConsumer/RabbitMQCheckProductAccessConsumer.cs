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
    public class RabbitMQCheckProductAccessConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<RabbitMQCheckProductAccessConsumer> logger;
        private readonly IRabbitMQSender rabbitMQSender;
        private IConnection connection;
        private IModel channel;

        public RabbitMQCheckProductAccessConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory scopeFactory, ILogger<RabbitMQCheckProductAccessConsumer> logger,
            IRabbitMQSender rabbitMQSender)
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
            channel.QueueDeclare(queue: "CheckProductAccessQueue", false, false, false, arguments: null);

            this.scopeFactory = scopeFactory;
            this.logger = logger;
            this.rabbitMQSender = rabbitMQSender;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received message from queue: CheckProductAccessQueue");
                var prop = channel.CreateBasicProperties();
                prop.CorrelationId = args.BasicProperties.CorrelationId;
                try
                {
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    CheckAccessDto? data = JsonConvert.DeserializeObject<CheckAccessDto>(content);
                    var result = HandleMessage(data).GetAwaiter().GetResult();
                    rabbitMQSender.SendMessage(result, args.BasicProperties.ReplyTo, prop);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    rabbitMQSender.SendMessage(null, args.BasicProperties.ReplyTo, prop);
                }

                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("CheckProductAccessQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task<bool> HandleMessage(CheckAccessDto? data)
        {
            if (data is null) return false;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var accessRepository = scope.ServiceProvider.GetRequiredService<IAccessRepository>();
                if (data.GameId == Guid.Empty) return await accessRepository.CheckUserHasGame(data.ProductId, data.UserId);
                return await accessRepository.CheckUserHasProduct(data.ProductId, data.GameId, data.UserId);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}