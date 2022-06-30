using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProdcutsService_API.Messages;
using ProdcutsService_API.RabbitMQSender;
using ProductsService_API.Helpers;
using ProductsService_API.Repository.IRepository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace ProductsService_API.RabbitMQConsumer
{
    public class RabbitMQDeleteGameConsumer : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQDeleteGameConsumer> logger;

        public RabbitMQDeleteGameConsumer(IOptions<RabbitMQSettings> options, IRabbitMQSender rabbitMQSender,
            IServiceScopeFactory serviceScopeFactory, ILogger<RabbitMQDeleteGameConsumer> logger)
        {
            this.rabbitMQSender = rabbitMQSender;
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password,
                Port = options.Value.Port
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "DeleteGameProductQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                try
                {
                    logger.LogInformation("Received message from queue: DeleteGameProductQueue");
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    Guid gameId = JsonConvert.DeserializeObject<Guid>(content);
                    HandleMessage(gameId).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("DeleteGameProductQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(Guid data)
        {
            if (data != Guid.Empty)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var gamesRepository = scope.ServiceProvider.GetRequiredService<IGamesRepository>();

                var game = await gamesRepository.FindGame(data);
                var isDeleted = await gamesRepository.DeleteGame(data);
                if (isDeleted && game is not null) rabbitMQSender.SendMessage(new DeleteGameDto(game.GameId, game.Products.Select(x => x.Id)), "DeleteGameAccessQueue");
                return;
            }
        }
    }
}