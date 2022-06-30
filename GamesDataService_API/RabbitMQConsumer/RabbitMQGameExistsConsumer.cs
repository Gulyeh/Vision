using GamesDataService_API.Helpers;
using GamesDataService_API.RabbitMQSender;
using GamesDataService_API.Repository.IRepository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace GamesDataService_API.RabbitMQConsumer
{
    public class RabbitMQGameExistsConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<RabbitMQGameExistsConsumer> logger;
        private readonly IRabbitMQSender rabbitMQSender;
        private IConnection connection;
        private IModel channel;

        public RabbitMQGameExistsConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory scopeFactory, ILogger<RabbitMQGameExistsConsumer> logger,
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
            channel.QueueDeclare(queue: "CheckGameExistsQueue", false, false, false, arguments: null);

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
                logger.LogInformation("Received message from queue: CheckGameExistsQueue");
                var prop = channel.CreateBasicProperties();
                prop.CorrelationId = args.BasicProperties.CorrelationId;
                try
                {
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    Guid? gameId = JsonConvert.DeserializeObject<Guid>(content);
                    var result = HandleMessage(gameId).GetAwaiter().GetResult();
                    rabbitMQSender.SendMessage(result, args.BasicProperties.ReplyTo, prop);
                }
                catch (Exception ex)
                {
                    rabbitMQSender.SendMessage(null, args.BasicProperties.ReplyTo, prop);
                    logger.LogError(ex.ToString());
                }

                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("CheckGameExistsQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task<bool> HandleMessage(Guid? gameId)
        {
            if (gameId == Guid.Empty || gameId is null) return false;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var gamesRepository = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                return await gamesRepository.CheckGame((Guid)gameId);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}